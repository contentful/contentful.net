using Contentful.Core.Configuration;
using Contentful.Core.Configuration.Attributes;
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

            await Task.Delay(Delay);

            if (Responses.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await Task.FromResult(Responses.Dequeue());
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(Response);
        }
        public Action<HttpRequestMessage> VerifyRequest { get; set; }
        public Action VerificationBeforeSend { get; set; }
        public Queue<HttpResponseMessage> Responses { get; set; }
        public HttpResponseMessage Response { get; set; }
        public int Delay { get; set; }
    }
    
    public class TwoAssets
    {
        public Asset First { get; set; }

        public Asset Second { get; set; }
    }

    public class TestEntryModel : IMarker
    {
        public SystemProperties Sys { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }

    public class RichTextModel  : IContent
    {
        public SystemProperties Sys { get; set; }
        public string Title { get; set; }
        public Document RichText { get; set; }
        public string Body { get; set; }
    }

    public class TestCategory : IMarker, IContent
    {
        public string Title { get; set; }
    }

    public class TestWithTags
    {
        public SystemProperties Sys { get; set; }
        public List<string> Tags { get; set; }
    }

    public class TestCompany : IMarker
    {
        public string CompanyDescription { get; set; }
    }

    public class TestModelWithIncludes
    {
        public string Title { get; set; }
        public string Slug { get; set; }


        public Asset FeaturedImage { get; set; }
        public List<Author> Author { get; set; }
        public List<Category> Category { get; set; }
    }

    public class TestModelWithIncludesInterface
    {
        public string Title { get; set; }
        public string Slug { get; set; }


        public Asset FeaturedImage { get; set; }
        public List<IMarker> Author { get; set; }
        public List<Category> Category { get; set; }
        public IMarker Pop { get; set; }
    }

    public class Author : IMarker
    {
        public SystemProperties SystemProperties { get; set; }
 
        public string Name { get; set; }

        [JsonProperty(PropertyName = "NotCamelISay")]
        public string NotCamel { get; set; }

        [JsonProperty(PropertyName = "NoCamelHere")]
        public string NotACamelEither { get; set; }

        [JsonProperty(PropertyName = "long")]
        public string LongThing { get; set; }

        public Asset ProfilePhoto { get; set; }
        public List<TestModelWithIncludes> CreatedEntries { get; set; }

        [QueryField]
        public TestNested Test { get; set; }
    }

    public class CamelTest
    {
        public SystemProperties SystemProperties { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "NotCamelISay")]
        public string NotCamel { get; set; }

        [JsonProperty(PropertyName = "NoCamelHere")]
        public string NotACamelEither { get; set; }

        [JsonProperty(PropertyName = "long")]
        public string LongThing { get; set; }
    }

    public class Category
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public Asset Icon { get; set; }
    }

    public class TestNested : IMarker
    {
        public SystemProperties Sys { get; set; }
        public string Field1 { get; set; }
        public Asset Field4 { get; set; }
        public string NewField { get; set; }
        [QueryField]
        public Category Cat { get; set; }
    }

    public class ContentfulEvent : IMarker
    {
        public string Title { get; set; }
        public Asset Image { get; set; } = new Asset { File = new Core.Models.File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    }

    public class Group
    {
        public Asset Image { get; set; } = new Asset { File = new Core.Models.File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    }

    public class MainContainer
    {
        public List<Container> Items { get; set; }
    }

    public class MainContainerArray
    {
        public Container[] Items { get; set; }
    }

    public class Container
    {
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public Asset Media { get; set; }
    }


    public class TestNestedSharedItem : IMarker
    {
        public TestNested Shared { get; set; }
    }

    public class SelfReferencer
    {
        public SystemProperties Sys { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public List<SelfReferencer> SubCategories { get; set; }
    }

    public class SelfReferencerInArray
    {
        public SystemProperties Sys { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public SelfReferencer[] SubCategories { get; set; }
    }

    public class Footer
    {
        public string Key { get; set; }
    }

    public interface IMarker
    {

    }

    public class TestResolver : IContentTypeResolver
    {
        public Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { "ContentTypeId1", typeof(TestCategory) },
            { "ContentType2", typeof(TestCategory) },
            { "ContentType3", typeof(TestCategory) },
            { "testagain", typeof(TestCategory) },
            { "6XwpTaSiiI2Ak2Ww0oi6qa", typeof(TestCategory) },
            { "2PqfXUJwE8qSYKuM0U6w8M", typeof(TestEntryModel) },
            { "sFzTZbSuM8coEwygeUYes", typeof(TestCompany) },
            { "events", typeof(ContentfulEvent) },
            { "page", typeof(TestNestedSharedItem) }
        };

        public Type Resolve(string contentTypeId)
        {
            return _types.TryGetValue(contentTypeId, out var type) ? type : null;
        }
    }

    public class RichTextResolver : IContentTypeResolver
    {
        public Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { "embedded", typeof(RichTextModel) }
        };

        public Type Resolve(string contentTypeId)
        {
            return _types.TryGetValue(contentTypeId, out var type) ? type : null;
        }
    }

    public class ManagementEntry
    {
        public SystemProperties Sys { get; set; }
        public Dictionary<string, string> Field1 { get; set; }
        public Dictionary<string, string> Field34 { get; set; }
        public Dictionary<string, Document> RichText { get; set; }
    }
}
