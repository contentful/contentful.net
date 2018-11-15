using Contentful.AspNetCore.MiddleWare;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Contentful.AspNetCore.Tests.MiddleWare
{
    public class WebhookMiddlewareTests
    {
        [Fact]
        public async Task RegisteredConsumerShouldFireForCorrectTopic()
        {
            //Arrange
            var consumerBuilder = new ConsumerBuilder();
            var called = false;
            consumerBuilder.AddConsumer<Entry<dynamic>>("Test", SystemWebhookTopics.Entry, SystemWebhookActions.Publish, (s) => { called = true; return new { Result = "Ok" }; });
            var middleware = new WebhookMiddleware(null, consumerBuilder.Build());
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Contentful-Topic", new Microsoft.Extensions.Primitives.StringValues("ContentManagement.Entry.publish"));
            context.Request.Headers.Add("X-Contentful-Webhook-Name", new Microsoft.Extensions.Primitives.StringValues("Test"));
            context.Request.ContentType = "application/vnd.contentful.management.v1+json";
            //Act
            await middleware.Invoke(context);

            //Assert
            Assert.True(called);
        }

        [Fact]
        public async Task RegisteredConsumerShouldDeserializePayloadCorrectly()
        {
            //Arrange
            var consumerBuilder = new ConsumerBuilder();
            var called = false;
            var hookEntry = new Entry<dynamic>();
            consumerBuilder.AddConsumer<Entry<dynamic>>("Test", SystemWebhookTopics.Entry, SystemWebhookActions.Publish, (s) => { called = true; hookEntry = s; return new { Result = "Ok" }; });
            var middleware = new WebhookMiddleware(null, consumerBuilder.Build());
            var context = new DefaultHttpContext();

            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "123"
                },
                Fields = new
                {
                    Title = new Dictionary<string, string>
                    {
                        { "en-US", "Hello" },
                        { "sv-SE", "Hallå" }
                    }
                }
            };
            
            string jsonObject = JsonConvert.SerializeObject(entry);

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(jsonObject);
            writer.Flush();
            memoryStream.Position = 0;

            context.Request.Body = memoryStream;
            context.Request.Headers.Add("X-Contentful-Topic", new Microsoft.Extensions.Primitives.StringValues("ContentManagement.Entry.publish"));
            context.Request.Headers.Add("X-Contentful-Webhook-Name", new Microsoft.Extensions.Primitives.StringValues("Test"));
            context.Request.ContentType = "application/vnd.contentful.management.v1+json";
            //Act
            await middleware.Invoke(context);

            //Assert
            Assert.True(called);
            Assert.Equal("123", hookEntry.SystemProperties.Id);
            Assert.Equal("Hello", hookEntry.Fields.Title["en-US"].ToString());
        }

        [Fact]
        public async Task RegisteredConsumerShouldDeserializePayloadCorrectlyForMultipleConsumers()
        {
            //Arrange
            var consumerBuilder = new ConsumerBuilder();
            var called = false;
            var secondCalled = false;
            var hookEntry = new Entry<dynamic>();
            var secondEntry = new Entry<dynamic>();
            consumerBuilder.AddConsumer<Entry<dynamic>>("Test", SystemWebhookTopics.Entry, SystemWebhookActions.Publish, (s) => { called = true; hookEntry = s; return new { Result = "Ok" }; });
            consumerBuilder.AddConsumer<Entry<dynamic>>("Test", SystemWebhookTopics.Entry, SystemWebhookActions.Publish, (s) => { secondCalled = true; secondEntry = s; return new { Result = "Ok" }; });
            var middleware = new WebhookMiddleware(null, consumerBuilder.Build());
            var context = new DefaultHttpContext();

            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "123"
                },
                Fields = new
                {
                    Title = new Dictionary<string, string>
                    {
                        { "en-US", "Hello" },
                        { "sv-SE", "Hallå" }
                    }
                }
            };

            string jsonObject = JsonConvert.SerializeObject(entry);

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(jsonObject);
            writer.Flush();
            memoryStream.Position = 0;

            context.Request.Body = memoryStream;
            context.Request.Headers.Add("X-Contentful-Topic", new Microsoft.Extensions.Primitives.StringValues("ContentManagement.Entry.publish"));
            context.Request.Headers.Add("X-Contentful-Webhook-Name", new Microsoft.Extensions.Primitives.StringValues("Test"));
            context.Request.ContentType = "application/vnd.contentful.management.v1+json";
            //Act
            await middleware.Invoke(context);

            //Assert
            Assert.True(called);
            Assert.Equal("123", hookEntry.SystemProperties.Id);
            Assert.Equal("Hello", hookEntry.Fields.Title["en-US"].ToString());
            Assert.True(secondCalled);
            Assert.Equal("123", secondEntry.SystemProperties.Id);
            Assert.Equal("Hello", secondEntry.Fields.Title["en-US"].ToString());
        }
    }
}
