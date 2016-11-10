using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Options;
using Xunit;
using System.Net;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using File = System.IO.File;

namespace Contentful.Core.Tests
{
    public class ContentfulClientTests
    {
        private ContentfulClient _client;
        private FakeMessageHandler _handler;

        public ContentfulClientTests()
        {
            _handler = new FakeMessageHandler();
            var httpClient = new HttpClient(_handler);
            _client = new ContentfulClient(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "123",
                SpaceId = "666",
                UsePreviewApi = false
            }));
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseToArbitraryModelCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleEntry.json");
            
            //Act
            var res = await _client.GetEntryAsync<TestEntryModel>("12");

            //Assert
            Assert.Equal("SoSo Wall Clock", res.ProductName);
            Assert.Equal("soso-wall-clock", res.Slug);
        }

        [Fact]
        public async Task GetEntryWithInvalidAccessTokenShouldSerializeErrorMessageCorrectlyAndThrowContentfulException()
        {
            //Arrange
            var response = GetResponseFromFile(@"JsonFiles\ErrorInvalidToken.json");
            response.StatusCode = HttpStatusCode.Unauthorized;
            _handler.Response = response;
            //Act

            //Assert
            await Assert.ThrowsAsync<ContentfulException>(async () => await _client.GetEntryAsync<TestEntryModel>("12"));
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseCorrectlyIntoAnEntryModel()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleEntry.json");

            //Act
            var res = await _client.GetEntryAsync<Entry<TestEntryModel>>("12");

            //Assert
            Assert.Equal("SoSo Wall Clock", res.Fields.ProductName);
            Assert.Equal("soso-wall-clock", res.Fields.Slug);
            Assert.Equal(DateTime.Parse("2016-11-03T10:50:05.033Z").ToUniversalTime(), res.SystemProperties.CreatedAt);
            Assert.Equal("2PqfXUJwE8qSYKuM0U6w8M", res.SystemProperties.ContentType.SystemProperties.Id);
        }

        [Fact]
        public async Task GetEntryShouldThrowArgumentExceptionIfNoEntryIdIsProvided()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleEntry.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEntryAsync<TestEntryModel>(""));
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryType()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EntriesCollection.json");

            //Act
            var res = await _client.GetEntriesAsync<TestEntryModel>();

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Title);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfEntry()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EntriesCollection.json");

            //Act
            var res = await _client.GetEntriesAsync<Entry<TestEntryModel>>(queryBuilder:null);

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Fields.Title);
        }

        [Fact]
        public async Task GetEntriesByTypeShouldAddCorrectFilter()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EntriesCollection.json");
            var builder = new QueryBuilder();
            //Act
            var res = await _client.GetEntriesByType<TestEntryModel>("666", builder);

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Title);
            Assert.Equal("?content_type=666", builder.Build());
        }

        [Fact]
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EntriesCollection.json");

            //Act
            var res = await _client.GetEntriesCollectionAsync<Entry<TestEntryModel>>();

            //Assert
            Assert.Equal(9, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(9, res.Items.Count());
            Assert.Equal("Home & Kitchen", res.Items.First().Fields.Title);
            Assert.Equal(DateTime.Parse("2016-11-03T10:50:05.899Z").ToUniversalTime(), res.Items.First().SystemProperties.CreatedAt);
            Assert.Equal("6XwpTaSiiI2Ak2Ww0oi6qa", res.Items.First().SystemProperties.ContentType.SystemProperties.Id);
        }

        [Fact]
        public async Task GetAssetByIdShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleAsset.json");

            //Act
            var res = await _client.GetAssetAsync("12");

            //Assert
            Assert.Equal("ihavenoidea", res.Title);
            Assert.Null(res.Description);
        }

        [Fact]
        public async Task GetAssetByIdShouldThrowArgumentExceptionIfNoAssetIdIsProvided()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleAsset.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetAssetAsync(""));
        }

        private HttpResponseMessage GetResponseFromFile(string file)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(File.ReadAllText(file));
            return response;
        }
    }

    public class FakeMessageHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Response);
        }

        public HttpResponseMessage Response { get; set; }
    }

    public class TestEntryModel
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }
}
