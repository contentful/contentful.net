using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contentful.Core.Tests
{
    public abstract class ClientTestsBase
    {

        protected HttpResponseMessage GetResponseFromFile(string file)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var response = new HttpResponseMessage();
            var resources = assembly.GetManifestResourceNames();
            var resourceName = resources.FirstOrDefault(f => f.Equals($"Contentful.Core.Tests.JsonFiles.{file}" , StringComparison.OrdinalIgnoreCase));
            string json = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            response.Content = new StringContent(json);
            return response;
        }
    }

    public class FakeMessageHandler : HttpClientHandler
    {
        public FakeMessageHandler()
        {
            Responses = new Queue<HttpResponseMessage>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            VerifyRequest?.Invoke(request);
            VerificationBeforeSend?.Invoke();

            if (Responses.Count > 0)
            {
                return await Task.FromResult(Responses.Dequeue());
            }

            return await Task.FromResult(Response);
        }
        public Action<HttpRequestMessage> VerifyRequest { get; set; }
        public Action VerificationBeforeSend { get; set; }
        public Queue<HttpResponseMessage> Responses { get; set; }
        public HttpResponseMessage Response { get; set; }
    }

    public class TestEntryModel
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }

    public class TestEntryWithSysProperties : TestEntryModel
    {
        public SystemProperties Sys { get; set; }
    }

    public class TestModelWithIncludes
    {
        public string Title { get; set; }
        public string Slug { get; set; }


        public Asset FeaturedImage { get; set; }
        public List<Author> Author { get; set; }
    }

    [JsonConverter(typeof(EntryFieldJsonConverter))]
    public class Author
    {
        public SystemProperties SystemProperties { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "long")]
        public string LongThing { get; set; }
    }
}
