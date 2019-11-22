using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using System.Linq;
using Contentful.Core.Models;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace Contentful.AspNetCore.MiddleWare
{
    /// <summary>
    /// Middleware to handle Contentful webhooks.
    /// </summary>
    public class WebhookMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILookup<Tuple<string, string, string>, Tuple<Delegate, Func<HttpContext, bool>>> _consumers;
        private readonly Func<HttpContext, bool> _webhookAuthorization;

        /// <summary>
        /// Creates a new instance of WebhookMiddleware.
        /// </summary>
        /// <param name="next">The next delegate to be called in the middleware pipeline.</param>
        /// <param name="consumers">The webhook consumers to be configured.</param>
        public WebhookMiddleware(RequestDelegate next, ILookup<Tuple<string, string, string>, Tuple<Delegate, Func<HttpContext, bool>>> consumers): this(next, consumers, null)
        {
        }

        /// <summary>
        /// Creates a new instance of WebhookMiddleware.
        /// </summary>
        /// <param name="next">The next delegate to be called in the middleware pipeline.</param>
        /// <param name="consumers">The webhook consumers to be configured.</param>
        /// <param name="webhookAuthorization">The authorization to use for the webhooks.</param>
        public WebhookMiddleware(RequestDelegate next, ILookup<Tuple<string, string, string>, Tuple<Delegate, Func<HttpContext, bool>>> consumers, Func<HttpContext, bool> webhookAuthorization)
        {
            _next = next;
            _consumers = consumers;
            _webhookAuthorization = webhookAuthorization;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HttpContext in which the middleware is called.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;

            if (headers.ContainsKey("X-Contentful-Topic") == false || context.Request.ContentType != "application/vnd.contentful.management.v1+json")
            {
                await _next(context);
                return;
            }

            if(_webhookAuthorization != null)
            {
                if(_webhookAuthorization.Invoke(context) == false)
                {
                    //authorization failed.
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Request failed configured authorization.");
                    return;
                }
            }

            var topic = "";
            var webhookName = "";

            topic = headers.GetCommaSeparatedValues("X-Contentful-Topic")?.FirstOrDefault();
            webhookName = headers.GetCommaSeparatedValues("X-Contentful-Webhook-Name")?.FirstOrDefault();

            var foundConsumers = _consumers?.FirstOrDefault(c => (c.Key.Item1 == "*" || c.Key.Item1 == webhookName) &&
            topic.TopicMatches(c.Key.Item2, c.Key.Item3));

            if (foundConsumers == null || foundConsumers.Any() == false)
            {
                //The webhook was called correctly, but no consumers were configured.
                //return 202 accepted
                context.Response.StatusCode = 202;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Request accepted but no consuming code configured for webhook.");
                return;
            }

            var responses = new List<object>();

            var bodyString = "";

            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyString = await reader.ReadToEndAsync();
            }

                foreach (var consumer in foundConsumers.Select(c => c))
            {
                var methodInfo = consumer.Item1.GetType().GetMethod("Invoke");
                var param = methodInfo.GetParameters().First();
                var type = param.ParameterType;

                if (consumer.Item2 != null && consumer.Item2.Invoke(context) == false)
                {
                    responses.Add(new { HttpStatus = 401, HttpResponse = "Request failed pre-request authorization." });
                    continue;
                }

                var serializedObject = new object();
                var serializationSuccessful = false;
                try
                {
                    serializedObject = JsonConvert.DeserializeObject(bodyString, type);
                    serializationSuccessful = true;
                }
                catch (Exception ex)
                {
                    //Add the exception to the responses sent back to Contentful.
                    responses.Add(ex);
                }

                if (serializationSuccessful)
                {
                    var returnedObject = new object();
                    try
                    {
                        returnedObject = consumer.Item1.DynamicInvoke(serializedObject);
                        responses.Add(returnedObject);
                    }
                    catch (Exception ex)
                    {
                        //Add the exception to the responses sent back to Contentful.
                        responses.Add(ex);
                    }
                }
                
            }

            context.Response.StatusCode = 200;

            if (responses.Any(c => c is Exception))
            {
                //we got one or more exceptions. Set status accordingly.
                context.Response.StatusCode = 500;
            }
            var serializedList = JsonConvert.SerializeObject(responses);

            await context.Response.WriteAsync(serializedList);
        }
    }

    /// <summary>
    /// Interface for building a number of webhook consumers.
    /// </summary>
    public interface IConsumerBuilder
    {
        /// <summary>
        /// The authorization for the middleware. Will be called before any consumer is executed.
        /// </summary>
        Func<HttpContext, bool> WebhookAuthorization { get; set; }

        /// <summary>
        /// Adds a consumer to the middleware pipeline.
        /// </summary>
        /// <typeparam name="T">The type the consumer expects.</typeparam>
        /// <param name="name">The name of the webhook. * can be used as a wildcard.</param>
        /// <param name="topicType">The type of topics to trigger this consumer for. * can be used as a wildcard.</param>
        /// <param name="topicAction">The actions to trigger this consumer for. * can be used as a wildcard.</param>
        /// <param name="consumer">The consumer that will be called if the name, action and topic matches.</param>
        /// <param name="preRequestVerification">Consumer specific verification of the request.</param>
        void AddConsumer<T>(string name, string topicType, string topicAction, Func<T, object> consumer, Func<HttpContext, bool> preRequestVerification = null);
    }

    /// <summary>
    /// Builds a number of consumers for the webhook middleware.
    /// </summary>
    public class ConsumerBuilder : IConsumerBuilder
    {
        private readonly List<Tuple<Tuple<string, string, string>, Delegate, Func<HttpContext, bool>>> _consumers = new List<Tuple<Tuple<string, string, string>, Delegate, Func<HttpContext, bool>>>();

        /// <summary>
        /// The authorization for the middleware. Will be called before any consumer is executed.
        /// </summary>
        public Func<HttpContext, bool> WebhookAuthorization { get; set; }

        /// <summary>
        /// Adds a consumer to the middleware pipeline.
        /// </summary>
        /// <typeparam name="T">The type the consumer expects.</typeparam>
        /// <param name="name">The name of the webhook. * can be used as a wildcard.</param>
        /// <param name="topicType">The type of topics to trigger this consumer for. * can be used as a wildcard.</param>
        /// <param name="topicAction">The actions to trigger this consumer for. * can be used as a wildcard.</param>
        /// <param name="consumer">The consumer that will be called if the name, action and topic matches.</param>
        /// <param name="preRequestVerification">Consumer specific verification of the request.</param>
        public void AddConsumer<T>(string name, string topicType, string topicAction, Func<T, object> consumer, Func<HttpContext, bool> preRequestVerification = null)
        {
            var tuple = Tuple.Create(name, topicType, topicAction);
            Delegate consumerDelegate = consumer;
            _consumers.Add(Tuple.Create(tuple, consumerDelegate, preRequestVerification));
        }
        
        /// <summary>
        /// Helper method to turn the list of consumers into a lookup.
        /// </summary>
        /// <returns>The lookup.</returns>
        public ILookup<Tuple<string, string, string>, Tuple<Delegate, Func<HttpContext, bool>>> Build()
        {
            return _consumers.ToLookup(x => x.Item1, x => Tuple.Create(x.Item2, x.Item3));
        }
    }

    /// <summary>
    /// Extensions for the <see cref="WebhookMiddleware"/>.
    /// </summary>
    public static class WebhookMiddlewareExtensions
    {
        /// <summary>
        /// Adds the Contentful webhook middleware to the middleware pipeline.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder.</param>
        /// <param name="configureConsumers">The consumers that will consume incoming webhook calls.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public static IApplicationBuilder UseContentfulWebhooks(this IApplicationBuilder builder,
            Action<IConsumerBuilder> configureConsumers)
        {
            var consumerBuilder = new ConsumerBuilder();

            configureConsumers(consumerBuilder);

            if(consumerBuilder.WebhookAuthorization == null)
            {
                consumerBuilder.WebhookAuthorization = s => true;
            }

            return builder.UseMiddleware<WebhookMiddleware>(consumerBuilder.Build(), consumerBuilder.WebhookAuthorization);
        }

        /// <summary>
        /// Helper method to verify wether a type and action matches a specific topic.
        /// </summary>
        /// <param name="topic">The topic to match.</param>
        /// <param name="topicType">The type to match to the topic.</param>
        /// <param name="topicAction">The action to match to the topic.</param>
        /// <returns>Wether the topic matches or not.</returns>
        public static bool TopicMatches(this string topic, string topicType, string topicAction)
        {
            if(topic == null)
            {
                return false;
            }

            var topicParts = topic.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var matchesType = false;
            var matchesAction = false;

            if (topicType == "*" || topicType.Equals(topicParts[1], StringComparison.OrdinalIgnoreCase))
            {
                matchesType = true;
            }

            if (topicAction == "*" || topicAction.Equals(topicParts[2], StringComparison.OrdinalIgnoreCase))
            {
                matchesAction = true;
            }

            return matchesType && matchesAction;
        }
    }
}
