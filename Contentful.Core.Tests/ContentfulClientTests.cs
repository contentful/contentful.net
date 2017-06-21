using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Xunit;
using System.Net;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using System.Threading;
using System.Reflection;

namespace Contentful.Core.Tests
{
    public class ContentfulClientTests : ClientTestsBase
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
        public async Task CreatingAContentfulClientAndMakingCallShouldAddAuthHeader()
        {
            //Arrange
            var httpClient = new HttpClient(_handler);
            var client = new ContentfulClient(httpClient, "444", "435");
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");
            var authHeader = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                authHeader = request.Headers.GetValues("Authorization").First();
            };
            //Act
            await client.GetAssetAsync("564");

            //Assert
            Assert.Equal("Bearer 444", authHeader);
        }

        [Fact]
        public async Task CreatingAContentfulClientAndMakingCallShouldAddUserAgentHeader()
        {

            //Arrange
            var httpClient = new HttpClient(_handler);
            var client = new ContentfulClient(httpClient, "123", "435");
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");
            var userAgent = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                userAgent = request.Headers.GetValues("X-Contentful-User-Agent").First();
            };
            var version = typeof(ContentfulClientBase).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

            //Act
            await client.GetAssetAsync("123");

            //Assert
            Assert.StartsWith($"sdk contentful.csharp/{version}", userAgent);
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseToArbitraryModelCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");
            
            //Act
            var res = await _client.GetEntryAsync<TestEntryModel>("12");

            //Assert
            Assert.Equal("SoSo Wall Clock", res.ProductName);
            Assert.Equal("soso-wall-clock", res.Slug);
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseToArbitraryModelWithSystemPropertiesCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");

            //Act
            var res = await _client.GetEntryAsync<TestEntryWithSysProperties>("12");

            //Assert
            Assert.Equal("SoSo Wall Clock", res.ProductName);
            Assert.Equal("soso-wall-clock", res.Slug);
            Assert.Equal("4BqrajvA8E6qwgkieoqmqO", res.Sys.Id);
            Assert.Equal(4, res.Sys.Revision);
            Assert.Equal("n9r7gd2bwvqt", res.Sys.Space.SystemProperties.Id);
        }

        [Fact]
        public async Task GetEntryWithInvalidAccessTokenShouldSerializeErrorMessageCorrectlyAndThrowContentfulException()
        {
            //Arrange
            var response = GetResponseFromFile(@"ErrorInvalidToken.json");
            response.StatusCode = HttpStatusCode.Unauthorized;
            _handler.Response = response;
            //Act

            //Assert
            await Assert.ThrowsAsync<ContentfulException>(async () => await _client.GetEntryAsync<TestEntryModel>("12"));
        }

        [Fact]
        public async Task RateLimitExceptionShouldBeThrownCorrectly()
        {
            //Arrange
            var response = GetResponseFromFile(@"ErrorRateLimit.json");
            response.StatusCode = (HttpStatusCode)429;
            response.Headers.Add("X-Contentful-RateLimit-Reset", "45");
            _handler.Response = response;
            //Act
            var ex = await Assert.ThrowsAsync<ContentfulRateLimitException>(async () => await _client.GetEntryAsync<TestEntryModel>("12"));
            //Assert
            Assert.Equal(45, ex.SecondsUntilNextRequest);
        }

        [Fact]
        public async Task RateLimitWithRetryShouldCallSeveralTimes()
        {
            //Arrange
            _handler = new FakeMessageHandler();
            var httpClient = new HttpClient(_handler);
            _client = new ContentfulClient(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "123",
                SpaceId = "666",
                UsePreviewApi = false,
                MaxNumberOfRateLimitRetries = 3
            });

            var response = GetResponseFromFile(@"ErrorRateLimit.json");
            response.StatusCode = (HttpStatusCode)429;
            response.Headers.Add("X-Contentful-RateLimit-Reset", "1");
            
            _handler.Response = response;
            var numberOfTimesCalled = 0;
            _handler.VerificationBeforeSend = () => { numberOfTimesCalled++; };
            _handler.VerifyRequest = (HttpRequestMessage msg) => { response.RequestMessage = msg; };  
            //Act
            var ex = await Assert.ThrowsAsync<ContentfulRateLimitException>(async () => await _client.GetEntryAsync<TestEntryModel>("12"));
            //Assert
            Assert.Equal(1, ex.SecondsUntilNextRequest);
            //1 request + 3 retries
            Assert.Equal(4, numberOfTimesCalled);
        }

        [Fact]
        public async Task RateLimitWithRetryShouldStopCallingOnSuccess()
        {
            //Arrange
            _handler = new FakeMessageHandler();
            var httpClient = new HttpClient(_handler);
            _client = new ContentfulClient(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "123",
                SpaceId = "666",
                UsePreviewApi = false,
                MaxNumberOfRateLimitRetries = 3
            });

            var response = GetResponseFromFile(@"ErrorRateLimit.json");
            response.StatusCode = (HttpStatusCode)429;
            response.Headers.Add("X-Contentful-RateLimit-Reset", "1");

            _handler.Response = response;
            var numberOfTimesCalled = 0;
            _handler.VerificationBeforeSend = () => { numberOfTimesCalled++; };
            _handler.VerifyRequest = (HttpRequestMessage msg) => { response.RequestMessage = msg; };

            _handler.Responses.Enqueue(response);
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntry.json"));
            //Act
            var res = await _client.GetEntryAsync<TestEntryModel>("12");
            //Assert
            //1 request + 1 retries
            Assert.Equal(2, numberOfTimesCalled);
            Assert.Equal("SoSo Wall Clock", res.ProductName);
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseCorrectlyIntoAnEntryModel()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");

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
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEntryAsync<TestEntryModel>(""));
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryType()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");

            //Act
            var res = await _client.GetEntriesAsync<TestEntryModel>();

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Title);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryTypeWithSystemProperties()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");

            //Act
            var res = await _client.GetEntriesAsync<TestEntryWithSysProperties>();
            var list = res.ToList();
            //Assert
            Assert.Equal(9, list.Count);
            Assert.Equal("Home & Kitchen", list.First().Title);
            Assert.Equal("5KsDBWseXY6QegucYAoacS", list[1].Sys.Id);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryTypeWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludes.json");

            //Act
            var res = await _client.GetEntriesAsync<TestModelWithIncludes>();
            var list = res.ToList();
            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal("AssetId4", list.Last().FeaturedImage.SystemProperties.Id);
            Assert.Equal("Alice in Wonderland", list.Last().FeaturedImage.Title);
            Assert.Equal("Mike Springer", list.First().Author.First().Name);
            Assert.Equal("Lewis Carroll", list.Last().Author.First().Name);
            Assert.Equal("Mike Springer", list.First().Author.First().ProfilePhoto.Title);
            Assert.Equal(1, list.First().Author.First().CreatedEntries.Count);
            Assert.Equal("contentful wyam logo", list.First().Author.First().Test.Field4.Title);
            Assert.Equal("Literature", list.Last().Category.First().Title);
        }

        [Fact]
        public async Task GetEntriesShouldSkipMissingIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludesMissingEntries.json");

            //Act
            var res = await _client.GetEntriesAsync<TestModelWithIncludes>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(0, list.First().Author.Count);
            Assert.Equal(0, list.First().Category.Count);

            Assert.Equal(0, list.Last().Author.Count);
            Assert.Equal(0, list.Last().Category.Count);

            Assert.Null(list.Last().FeaturedImage);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithNestedAsset()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"NestedAsset.json");

            //Act
            var res = await _client.GetEntriesAsync<MainContainer>();
            var list = res.ToList();

            //Assert
            Assert.NotNull(list[0].Items[0].Items[0].Media.File);
            Assert.Equal("test.container.item1.media", list[0].Items[0].Items[0].Media.Title);
            Assert.Equal(1, list.Count());
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithSameAssetLinkedMultipleTimes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"MultipleLinksSameAsset.json");

            //Act
            var res = await _client.GetEntriesAsync<TwoAssets>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(list[0].First, list[0].Second);
            Assert.Equal(list[0].First, list[1].Second);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithSameAssetLinkedMultipleTimesForEntryObjects()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"MultipleLinksSameAsset.json");

            //Act
            var res = await _client.GetEntriesAsync<Entry<TwoAssets>>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(list[0].Fields.First, list[0].Fields.Second);
            Assert.Equal(list[0].Fields.First, list[1].Fields.Second);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryTypeWithIncludesForAuthor()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludesAuthor.json");

            //Act
            var res = await _client.GetEntriesAsync<Author>();
            var list = res.ToList();
            //Assert
            Assert.Equal(1, list.Count);
            Assert.Equal("Lewis Carroll", list.Last().Name);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfEntry()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");

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
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");
            var builder = QueryBuilder<TestEntryModel>.New;
            //Act
            var res = await _client.GetEntriesByTypeAsync("666", builder);

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Title);
            Assert.Equal("?content_type=666", builder.Build());
        }

        [Fact]
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");

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
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollectionWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludes.json");

            //Act
            var res = await _client.GetEntriesCollectionAsync<Entry<TestModelWithIncludes>>();

            //Assert
            Assert.Equal(2, res.Count());
            Assert.Equal("Alice in Wonderland", res.IncludedAssets.Single(c => c.SystemProperties.Id == "AssetId4").Title);
            Assert.Equal("Alice in Wonderland", res.IncludedAssets.Single(c => c.SystemProperties.Id == "AssetId4").TitleLocalized["en-US"]);
            Assert.Equal("Lewis Carroll", res.IncludedEntries.Single(c => c.SystemProperties.Id == "EntryId6").Fields.name.ToString());
        }

        [Fact]
        public async Task GetEntriesCollectionWithAllLocalesShouldSerializeIntoCorrectCollectionWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludesAndLocales.json");

            //Act
            var res = await _client.GetEntriesCollectionAsync<Entry<dynamic>>();

            //Assert
            Assert.Equal(2, res.Count());
            Assert.Equal("Alice in Wonderland", res.IncludedAssets.Single(c => c.SystemProperties.Id == "AssetId4").TitleLocalized["en-US"]);
            Assert.Equal("Alice i underlandet", res.IncludedAssets.Single(c => c.SystemProperties.Id == "AssetId4").TitleLocalized["sv-SE"]);
            Assert.Equal("Lewis Carroll", res.IncludedEntries.Single(c => c.SystemProperties.Id == "EntryId66").Fields.name["en-US"].ToString());
        }

        [Fact]
        public async Task GetAssetByIdShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");

            //Act
            var res = await _client.GetAssetAsync("12");

            //Assert
            Assert.Equal("ihavenoidea", res.Title);
            Assert.Null(res.Description);
            Assert.NotNull(res.File);
        }

        [Fact]
        public async Task GetAssetByIdShouldThrowArgumentExceptionIfNoAssetIdIsProvided()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetAssetAsync(""));
        }

        [Fact]
        public async Task GetAssetsShouldSerializeCorrectlyToAnEnumerableOfAsset()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollection.json");

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
            _handler.Response = GetResponseFromFile(@"AssetsCollection.json");

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
            _handler.Response = GetResponseFromFile(@"SampleSpace.json");

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
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");

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
            _handler.Response = GetResponseFromFile(@"ContenttypesCollection.json");

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
            _handler.Response = GetResponseFromFile(@"InitialSyncNoNextPage.json");

            //Act
            var res = await _client.SyncInitialAsync();

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
            _handler.Response = GetResponseFromFile(@"InitialSyncDeletionOnly.json");

            //Act
            var res = await _client.SyncInitialAsync(SyncType.Deletion);

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
            _handler.Response = GetResponseFromFile(@"NextSyncUrl.json");

            //Act
            var res = await _client.SyncNextResultAsync("SomeSyncToken");

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
            _handler.Response = GetResponseFromFile(@"NextSyncUrl.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.SyncNextResultAsync(""));
            //Assert
            Assert.Equal($"nextPageUrl must be specified.{Environment.NewLine}Parameter name: nextSyncOrPageUrl", ex.Message);
        }

        [Fact]
        public async Task SyncInitialRecursiveShouldSyncAllPagesOfSyncIntoSingleSyncResult()
        {
            //Arrange
            _handler.Responses.Enqueue(GetResponseFromFile(@"InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"InitialSyncNextPagePresent.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"InitialSyncNoNextPage.json"));

            //Act
            var res = await _client.SyncInitialRecursiveAsync();

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

        [Fact]
        public void CancellingRequestShouldSuccesfulAbortRequest()
        {
            //Arrange
            var source = new CancellationTokenSource(1500);
            _handler.VerificationBeforeSend = async () => { await Task.Delay(3000); };
            //Act
            var ex = Assert.ThrowsAsync<OperationCanceledException>(async () => await _client.GetEntryAsync<Entry<dynamic>>("123", "", source.Token));

            //Assert
            Assert.Equal(TaskStatus.Faulted,ex.Status);
        }

        [Fact]
        public async Task UsingAContentTypeResolverShouldYieldCorrectTypesInCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");
            _client.ContentTypeResolver = new TestResolver();
            _client.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
            //Act
            var res = await _client.GetEntriesAsync<IMarker>();

            //Assert
            Assert.Equal(9, res.Count());
            Assert.IsType<TestCategory>(res.First());
        }

        [Fact]
        public async Task GetEntryWithInvalidSpaceShouldSerializeErrorMessageCorrectlyAndThrowContentfulException()
        {
            //Arrange
            var response = GetResponseFromFile(@"ErrorSpaceNotFound.json");
            response.StatusCode = HttpStatusCode.Unauthorized;
            _handler.Response = response;
            //Act

            //Assert
            await Assert.ThrowsAsync<ContentfulException>(async () => await _client.GetEntryAsync<TestEntryModel>("12"));
        }

        [Fact]
        public async Task AllAssetsInACollectionShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntryCollectionLoopedReferences.json");

            //Act
            _client.ResolveEntriesSelectively = true;
            var entries = await _client.GetEntriesAsync<ContentfulEvent>();
            _client.ResolveEntriesSelectively = false;
            var nulls = entries.Where(c => c.Image == null).ToList();

            //Assert
            Assert.Equal(4, entries.Count());
            Assert.Collection(entries, 
                c => { Assert.NotNull(c.Image); },
                c => { Assert.NotNull(c.Image); },
                c => { Assert.NotNull(c.Image); },
                c => { Assert.NotNull(c.Image); }
                );
        }
    }
}
