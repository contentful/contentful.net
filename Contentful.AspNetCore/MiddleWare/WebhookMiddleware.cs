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
    public class WebhookMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILookup<Tuple<string, string, string>, Delegate> _consumers;
        private readonly Func<HttpContext, bool> _webhookAuthorization;

        public WebhookMiddleware(RequestDelegate next, ILookup<Tuple<string, string, string>, Delegate> consumers, Func<HttpContext, bool> webhookAuthorization = null)
        {
            _next = next;
            _consumers = consumers;
            _webhookAuthorization = webhookAuthorization;
        }

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

            foreach (var consumer in foundConsumers.Select(c => c))
            {
                var methodInfo = consumer.GetType().GetMethod("Invoke");
                var param = methodInfo.GetParameters().First();
                var type = param.ParameterType;
                var body = context.Request.Body;

                using (var reader = new StreamReader(body))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var ser = new JsonSerializer();
                    var serializedObject = new object();
                    var serializationSuccessful = false;
                    try
                    {
                        serializedObject = ser.Deserialize(jsonReader, type);
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
                            returnedObject = consumer.DynamicInvoke(serializedObject);
                            responses.Add(returnedObject);
                        }
                        catch (Exception ex)
                        {
                            //Add the exception to the responses sent back to Contentful.
                            responses.Add(ex);
                        }
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

    public interface IConsumerBuilder
    {
        Func<HttpContext, bool> WebhookAuthorization { get; set; }
        void AddConsumer<T>(string name, string topicType, string topicAction, Func<T, object> consumer, Func<HttpContext, bool> preRequestVerification = null);
    }

    public class ConsumerBuilder : IConsumerBuilder
    {
        private readonly List<Tuple<Tuple<string, string, string>, Delegate>> _consumers = new List<Tuple<Tuple<string, string, string>, Delegate>>();

        public Func<HttpContext, bool> WebhookAuthorization { get; set; }

        public void AddConsumer<T>(string name, string topicType, string topicAction, Func<T, object> consumer, Func<HttpContext, bool> preRequestVerification = null)
        {
            var tuple = Tuple.Create(name, topicType, topicAction);
            Delegate d = consumer;
            _consumers.Add(Tuple.Create(tuple, d));
        }
        
        public ILookup<Tuple<string, string, string>, Delegate> Build()
        {
            return _consumers.ToLookup(x => x.Item1, x => x.Item2);
        }
    }

    public static class WebhookMiddlewareExtensions
    {
        public static IApplicationBuilder UseContentfulWebhooks(this IApplicationBuilder builder,
            Action<IConsumerBuilder> configureConsumers)
        {
            var consumerBuilder = new ConsumerBuilder();

            configureConsumers(consumerBuilder);

            return builder.UseMiddleware<WebhookMiddleware>(consumerBuilder.Build(), consumerBuilder.WebhookAuthorization);
        }

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
