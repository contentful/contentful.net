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
using Newtonsoft.Json;

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
            _client = new ContentfulClient(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "123",
                SpaceId = "666",
                UsePreviewApi = false
            });
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
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryTypeWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EntriesCollectionWithIncludes.json");

            //Act
            var res = await _client.GetEntriesAsync<TestModelWithIncludes>();

            //Assert
            Assert.Equal(2, res.Count());
            Assert.Equal("AssetId4", res.Last().FeaturedImage.SystemProperties.Id);
            Assert.Equal("Mike Springer", res.First().Author.First().Fields.Name);
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
            var res = await _client.GetEntriesByTypeAsync<TestEntryModel>("666", builder);

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

        [Fact]
        public async Task GetAssetsShouldSerializeCorrectlyToAnEnumerableOfAsset()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\AssetsCollection.json");

            //Act
            var res = await _client.GetAssetsAsync();

            //Assert
            Assert.Equal(12, res.Count());
            Assert.Equal("Playsam Streamliner", res.First().Title);
            Assert.Equal("Merchandise photo", res.First().Description);
        }

        [Fact]
        public async Task GetAssetsCollectionShouldSerializeIntoCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\AssetsCollection.json");

            //Act
            var res = await _client.GetAssetsCollectionAsync(queryBuilder:null);

            //Assert
            Assert.Equal(12, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(12, res.Items.Count());
            Assert.Equal("Playsam Streamliner", res.Items.First().Title);
            Assert.Equal("Merchandise photo", res.Items.First().Description);
            Assert.Equal(DateTime.Parse("2016-11-03T10:49:56.838Z").ToUniversalTime(), res.Items.First().SystemProperties.CreatedAt);
            Assert.Equal("n9r7gd2bwvqt", res.Items.First().SystemProperties.Space.SystemProperties.Id);
        }

        [Fact]
        public async Task GetSpaceShouldSerializeIntoCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleSpace.json");

            //Act
            var res = await _client.GetSpaceAsync();

            //Assert
            Assert.Equal("Products", res.Name);
            Assert.Equal("n9r7gd2bwvqt", res.SystemProperties.Id);
            Assert.Equal("Space", res.SystemProperties.Type);
            Assert.Collection(res.Locales,(l) => {
                Assert.Equal("en-US", l.Code);
                Assert.True(l.Default);
                Assert.Equal("U.S. English", l.Name);
                Assert.Null(l.FallbackCode);
            }, (l) =>
            {
                Assert.Equal("sv", l.Code);
                Assert.False(l.Default);
                Assert.Equal("Swedish", l.Name);
                Assert.Equal("en-US", l.FallbackCode);
            });
           
        }

        [Fact]
        public async Task GetContentTypeShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleContentType.json");

            //Act
            var res = await _client.GetContentTypeAsync("someid");

            //Assert
            Assert.Equal("Product", res.Name);
            Assert.Equal("productName", res.DisplayField);
            Assert.Equal(12, res.Fields.Count);
            Assert.True(res.Fields[0].Localized);
            Assert.Equal("Description", res.Fields[2].Name);
            Assert.Equal("Link", res.Fields[4].Items.Type);

        }

        [Fact]
        public async Task GetContentTypesShouldSerializeIntoAnEnumerableOfContentType()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\ContentTypesCollection.json");

            //Act
            var res = await _client.GetContentTypesAsync();

            //Assert
            Assert.Equal(3, res.Count());
            Assert.Equal("Brand", res.First().Name);
            Assert.Null(res.First().Description);
            Assert.Equal(DateTime.Parse("2016-11-03T10:49:52.260Z").ToUniversalTime(), res.First().SystemProperties.CreatedAt);
            Assert.Equal("n9r7gd2bwvqt", res.First().SystemProperties.Space.SystemProperties.Id);
            Assert.Equal(2, res.First().SystemProperties.Revision);
        }

        [Fact]
        public async Task SyncInitialShouldSerializeIntoSyncResultCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\InitialSyncNoNextPage.json");

            //Act
            var res = await _client.SyncInitial();

            //Assert
            Assert.Null(res.NextPageUrl);
            Assert.Equal("https://cdn.contentful.com/spaces/n9r7gd2bwvqt/sync?sync_token=sometoken", res.NextSyncUrl);
            Assert.Equal(12, res.Assets.Count());
            Assert.Equal(9, res.Entries.Count());
            Assert.Equal(0, res.DeletedAssets.Count());
            Assert.Equal(0, res.DeletedEntries.Count());
            Assert.Equal("4BqrajvA8E6qwgkieoqmqO", res.Entries.First().SystemProperties.Id);
            Assert.Equal("SoSo Wall Clock", res.Entries.First().Fields.productName["en-US"].ToString());
            Assert.Equal("SåSå Väggklocka", res.Entries.First().Fields.productName.sv.ToString());
        }

        [Fact]
        public async Task SyncInitialDeletionsShouldSerializeIntoSyncResultCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\InitialSyncDeletionOnly.json");

            //Act
            var res = await _client.SyncInitial(SyncType.Deletion);

            //Assert
            Assert.Null(res.NextPageUrl);
            Assert.Equal("https://cdn.contentful.com/spaces/n9r7gd2bwvqt/sync?sync_token=nono", res.NextSyncUrl);
            Assert.Equal(0, res.Assets.Count());
            Assert.Equal(0, res.Entries.Count());
            Assert.Equal(1, res.DeletedAssets.Count());
            Assert.Equal(0, res.DeletedEntries.Count());

        }

        [Fact]
        public async Task SyncNextUrlShouldSerializeIntoSyncResultCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\NextSyncUrl.json");

            //Act
            var res = await _client.SyncNextResult("SomeSyncToken");

            //Assert
            Assert.Null(res.NextPageUrl);
            Assert.Equal("https://cdn.contentful.com/spaces/n9r7gd2bwvqt/sync?sync_token=nexttoken", res.NextSyncUrl);
            Assert.Equal(1, res.Assets.Count());
            Assert.Equal(1, res.Entries.Count());
            Assert.Equal(0, res.DeletedAssets.Count());
            Assert.Equal(0, res.DeletedEntries.Count());
            Assert.Equal("6dbjWqNd9SqccegcqYq224", res.Entries.First().SystemProperties.Id);
            Assert.Equal("Whisk Beater - updated and improved!", res.Entries.First().Fields.productName["en-US"].ToString());
            Assert.Equal("Smisk slagare", res.Entries.First().Fields.productName.sv.ToString());
            Assert.Equal(34250, int.Parse(res.Assets.First().Fields.file["en-US"].details.size.ToString()));
        }

        [Fact]
        public async Task SyncNextUrlShouldThrowArgumentExceptionIfNoUrlIsProvided()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\NextSyncUrl.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.SyncNextResult(""));

        }

        [Fact]
        public async Task SyncInitialRecursiveShouldSyncAllPagesOfSyncIntoSingleSyncResult()
        {
            //Arrange
            _handler.Responses.Enqueue(GetResponseFromFile(@"JsonFiles\InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"JsonFiles\InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"JsonFiles\InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"JsonFiles\InitialSyncNoNextPage.json"));

            //Act
            var res = await _client.SyncInitialRecursive();

            //Assert
            Assert.Null(res.NextPageUrl);
            Assert.Equal("https://cdn.contentful.com/spaces/n9r7gd2bwvqt/sync?sync_token=sometoken", res.NextSyncUrl);
            Assert.Equal(48, res.Assets.Count());
            Assert.Equal(36, res.Entries.Count());
            Assert.Equal(0, res.DeletedAssets.Count());
            Assert.Equal(0, res.DeletedEntries.Count());
            Assert.Equal("4BqrajvA8E6qwgkieoqmqO", res.Entries.First().SystemProperties.Id);
            Assert.Equal("SoSo Wall Clock", res.Entries.First().Fields.productName["en-US"].ToString());
            Assert.Equal("SåSå Väggklocka", res.Entries.First().Fields.productName.sv.ToString());
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
        public FakeMessageHandler()
        {
            Responses = new Queue<HttpResponseMessage>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Responses.Count > 0)
            {
                return await Task.FromResult(Responses.Dequeue());
            }

            return await Task.FromResult(Response);
        }

        public Queue<HttpResponseMessage> Responses { get; set; }
        public HttpResponseMessage Response { get; set; }
    }

    public class TestEntryModel
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
    }

    public class TestModelWithIncludes
    {
        public string Title { get; set; }
        public string Slug { get; set; }

        
        public Asset FeaturedImage { get; set; }
        public IEnumerable<Entry<Author>> Author { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
    }
}
