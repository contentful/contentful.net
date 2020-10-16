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
using System.Text;
using System.Collections.Generic;

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
            var client = new ContentfulClient(httpClient, "444", "", "435");
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");
            var authHeader = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                authHeader = request.Headers.GetValues("Authorization").First();
            };
            //Act
            await client.GetAsset("564");

            //Assert
            Assert.Equal("Bearer 444", authHeader);
        }

        [Fact]
        public void CreatingAContentfulClientWithoutOptionsShouldThrowArgumentException()
        {
            //Arrange
            var httpClient = new HttpClient(_handler);

            //Act
            var ex = Assert.Throws<ArgumentException>(() => new ContentfulClient(httpClient, options: null));

            //Assert
            Assert.Equal($"The ContentfulOptions cannot be null.{Environment.NewLine}Parameter name: options", ex.Message);
        }

        [Fact]
        public async Task CreatingAContentfulClientAndMakingCallShouldAddUserAgentHeader()
        {
            //Arrange
            var httpClient = new HttpClient(_handler);
            var client = new ContentfulClient(httpClient, "123", "", "435");
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");
            var userAgent = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                userAgent = request.Headers.GetValues("X-Contentful-User-Agent").First();
            };
            var version = typeof(ContentfulClientBase).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

            //Act
            await client.GetAsset("123");

            //Assert
            Assert.StartsWith($"sdk contentful.csharp/{version}", userAgent);
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseToArbitraryModelCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");
            
            //Act
            var res = await _client.GetEntry<TestEntryModel>("12");

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
            var res = await _client.GetEntry<TestEntryModel>("12");

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
            await Assert.ThrowsAsync<ContentfulException>(async () => await _client.GetEntry<TestEntryModel>("12"));
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
            var ex = await Assert.ThrowsAsync<ContentfulRateLimitException>(async () => await _client.GetEntry<TestEntryModel>("12"));
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
            var ex = await Assert.ThrowsAsync<ContentfulRateLimitException>(async () => await _client.GetEntry<TestEntryModel>("12"));
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
            var res = await _client.GetEntry<TestEntryModel>("12");
            //Assert
            //1 request + 1 retries
            Assert.Equal(2, numberOfTimesCalled);
            Assert.Equal("SoSo Wall Clock", res.ProductName);
        }

        [Fact]
        public async Task GatewayTimeoutExceptionShouldBeThrownCorrectly()
        {
            //Arrange
            var response = new HttpResponseMessage();
            response.Content = new StringContent("");
            response.StatusCode = (HttpStatusCode)504;
            _handler.Response = response;
            //Act
            await Assert.ThrowsAsync<GatewayTimeoutException>(async () => await _client.GetEntry<TestEntryModel>("12"));
        }

        [Fact]
        public async Task SettingPreviewApiShouldUseCorrectApiKey()
        {
            //Arrange
            _handler = new FakeMessageHandler();
            var httpClient = new HttpClient(_handler);
            _client = new ContentfulClient(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "123",
                PreviewApiKey = "ABC-PREVIEW",
                SpaceId = "666",
                UsePreviewApi = true,
                MaxNumberOfRateLimitRetries = 3
            });
            var authHeader = "";
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                authHeader = request.Headers.GetValues("Authorization").First();
            };

            //Act
            var res = await _client.GetEntry<Entry<TestEntryModel>>("12");

            //Assert
            Assert.Equal("Bearer ABC-PREVIEW", authHeader);
        }

        [Fact]
        public async Task GetEntryShouldSerializeResponseCorrectlyIntoAnEntryModel()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");

            //Act
            var res = await _client.GetEntry<TestEntryModel>("12");

            //Assert
            Assert.Equal("SoSo Wall Clock", res.ProductName);
            Assert.Equal("soso-wall-clock", res.Slug);
            Assert.Equal(DateTime.Parse("2016-11-03T10:50:05.033Z").ToUniversalTime(), res.Sys.CreatedAt);
            Assert.Equal("2PqfXUJwE8qSYKuM0U6w8M", res.Sys.ContentType.SystemProperties.Id);
        }

        [Fact]
        public async Task GetEntryShouldThrowArgumentExceptionIfNoEntryIdIsProvided()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");

            //Act

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEntry<TestEntryModel>(""));
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryType()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");

            //Act
            var res = await _client.GetEntries<TestEntryModel>();

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
            var res = await _client.GetEntries<TestEntryModel>();
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
            var res = await _client.GetEntries<TestModelWithIncludes>();
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
            var res = await _client.GetEntries<TestModelWithIncludes>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(0, list.First().Author.Count);
            Assert.Equal(0, list.First().Category.Count);

            Assert.Equal(0, list.Last().Author.Count);
            Assert.Equal(0, list.Last().Category.Count);

            Assert.Null(list.Last().FeaturedImage);

            Assert.Collection(res.Errors,
                (e) => { Assert.Equal("AssetId4", e.Details.Id); },
                (e) => { Assert.Equal("EntryId2", e.Details.Id); },
                (e) => { Assert.Equal("EntryId3", e.Details.Id); },
                (e) => { Assert.Equal("EntryId6", e.Details.Id); },
                (e) => { Assert.Equal("EntryId4", e.Details.Id); }
                );
        }

        [Fact]
        public async Task GetEntriesShouldSkipMissingIncludesForInterface()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludesMissingEntries.json");

            //Act
            var res = await _client.GetEntries<TestModelWithIncludesInterface>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(0, list.First().Author.Count);
            Assert.Equal(0, list.First().Category.Count);

            Assert.Equal(0, list.Last().Author.Count);
            Assert.Equal(0, list.Last().Category.Count);

            Assert.Null(list.Last().FeaturedImage);

            Assert.Collection(res.Errors,
                (e) => { Assert.Equal("AssetId4", e.Details.Id); },
                (e) => { Assert.Equal("EntryId2", e.Details.Id); },
                (e) => { Assert.Equal("EntryId3", e.Details.Id); },
                (e) => { Assert.Equal("EntryId6", e.Details.Id); },
                (e) => { Assert.Equal("EntryId4", e.Details.Id); }
                );
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithNestedAsset()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"NestedAsset.json");

            //Act
            var res = await _client.GetEntries<MainContainer>();
            var list = res.ToList();

            //Assert
            Assert.NotNull(list[0].Items[0].Items[0].Media.File);
            Assert.Equal("test.container.item1.media", list[0].Items[0].Items[0].Media.Title);
            Assert.Equal(1, list.Count());
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithNestedAssetInArray()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"NestedAsset.json");

            //Act
            var res = await _client.GetEntries<MainContainerArray>();
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
            var res = await _client.GetEntries<TwoAssets>();
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
            var res = await _client.GetEntries<TwoAssets>();
            var list = res.ToList();

            //Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(list[0].First, list[0].Second);
            Assert.Equal(list[0].First, list[1].Second);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyToAnEnumerableOfArbitraryTypeWithIncludesForAuthor()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludesAuthor.json");

            //Act
            var res = await _client.GetEntries<Author>();
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
            var res = await _client.GetEntries<TestEntryModel>(queryBuilder:null);

            //Assert
            Assert.Equal(9, res.Count());
            Assert.Equal("Home & Kitchen", res.First().Title);
        }

        [Fact]
        public async Task GetEntriesByTypeShouldAddCorrectFilter()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");
            var builder = QueryBuilder<TestEntryModel>.New;
            //Act
            var res = await _client.GetEntriesByType("666", builder);

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
            var res = await _client.GetEntries<TestEntryModel>();

            //Assert
            Assert.Equal(9, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(9, res.Items.Count());
            Assert.Equal("Home & Kitchen", res.Items.First().Title);
            Assert.Equal(DateTime.Parse("2016-11-03T10:50:05.899Z").ToUniversalTime(), res.Items.First().Sys.CreatedAt);
            Assert.Equal("6XwpTaSiiI2Ak2Ww0oi6qa", res.Items.First().Sys.ContentType.SystemProperties.Id);
        }

        [Fact]
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollectionWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludes.json");

            //Act
            var res = await _client.GetEntries<Entry<TestModelWithIncludes>>();

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
            var res = await _client.GetEntries<Entry<dynamic>>();

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
            var res = await _client.GetAsset("12");

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
            await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetAsset(""));
        }

        [Fact]
        public async Task GetAssetsShouldSerializeCorrectlyToAnEnumerableOfAsset()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollection.json");

            //Act
            var res = await _client.GetAssets();

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
            var res = await _client.GetAssets(queryBuilder:null);

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
            var res = await _client.GetSpace();

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
            var res = await _client.GetContentType("someid");

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
            var res = await _client.GetContentTypes();

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
            _handler.Response = GetResponseFromFile(@"InitialSyncDeletionOnly.json");

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
            _handler.Response = GetResponseFromFile(@"NextSyncUrl.json");

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
            _handler.Response = GetResponseFromFile(@"NextSyncUrl.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.SyncNextResult(""));
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

        [Fact]
        public async Task CancellingRequestShouldSuccesfulAbortRequest()
        {
            //Arrange
            var source = new CancellationTokenSource(1500);
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");
            _handler.Delay = 3000;
            //Act
            var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () => await _client.GetEntry<Entry<dynamic>>("123", "", source.Token));

            //Assert
            Assert.Equal("The operation was canceled.",ex.Message);
        }

        [Fact]
        public async Task UsingAContentTypeResolverShouldYieldCorrectTypesInCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");
            _client.ContentTypeResolver = new TestResolver();
            //Act
            var res = await _client.GetEntries<IMarker>();

            //Assert
            Assert.Equal(9, res.Count());
            Assert.IsType<TestCategory>(res.First());
        }

        [Fact]
        public async Task UsingAContentTypeResolverShouldYieldCorrectTypesInCollectionWithIncludes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithIncludes.json");
            _client.ContentTypeResolver = new TestResolver();
            //Act
            var res = await _client.GetEntries<IMarker>();

            //Assert
            Assert.Equal(2, res.Count());
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
            await Assert.ThrowsAsync<ContentfulException>(async () => await _client.GetEntry<TestEntryModel>("12"));
        }

        [Fact]
        public async Task AllAssetsInACollectionShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntryCollectionLoopedReferences.json");

            //Act
            _client.ResolveEntriesSelectively = true;
            var entries = await _client.GetEntries<ContentfulEvent>();
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

        [Fact]
        public async Task AllAssetsInACollectionShouldDeserializeCorrectlyWithResolver()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntryCollectionLoopedReferences.json");
            _client.ContentTypeResolver = new TestResolver();

            //Act
            _client.ResolveEntriesSelectively = true;
            var entries = await _client.GetEntries<IMarker>();
            _client.ResolveEntriesSelectively = false;
            var nulls = entries.Where(c => (c as ContentfulEvent).Image == null).ToList();

            //Assert
            Assert.Equal(4, entries.Count());
            Assert.Collection(entries,
                c => { Assert.NotNull((c as ContentfulEvent).Image); },
                c => { Assert.NotNull((c as ContentfulEvent).Image); },
                c => { Assert.NotNull((c as ContentfulEvent).Image); },
                c => { Assert.NotNull((c as ContentfulEvent).Image); }
                );
        }

        [Fact]
        public async Task GetEntriesWithSelectShouldYieldCorrectResult()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithoutSys.json");
            var builder = QueryBuilder<TestWithTags>.New;
            //Act
            var res = await _client.GetEntriesByType("666", builder);

            //Assert
            Assert.Equal(4, res.Count());
            Assert.Equal("kitchen", res.First().Tags.First());
        }

        [Fact]
        public async Task ComplexStructureIsDeserializedCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"NestedSharedStructure.json");

            //Act
            _client.ResolveEntriesSelectively = true;
            var res = await _client.GetEntries<TestNestedSharedItem>();
            _client.ResolveEntriesSelectively = false;

            //Assert
            Assert.Equal(1, res.Count());
            Assert.NotNull(res.First().Shared);
            Assert.NotNull(res.First().Shared.Field1);
        }

        [Fact]
        public async Task ComplexStructureIsDeserializedCorrectlyWithResolver()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"NestedSharedStructure.json");
            _client.ContentTypeResolver = new TestResolver();

            //Act
            _client.ResolveEntriesSelectively = true;
            var res = await _client.GetEntries<IMarker>();
            _client.ResolveEntriesSelectively = false;

            //Assert
            Assert.Equal(1, res.Count());
            Assert.NotNull((res.First() as TestNestedSharedItem).Shared);
            Assert.NotNull((res.First() as TestNestedSharedItem).Shared.Field1);
        }

        [Fact]
        public async Task ComplexStructureWithSelfReferenceIsDeserializedCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithSelfreference.json");

            //Act
            var res = await _client.GetEntries<SelfReferencer>();

            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal(res.First().SubCategories.First().Sys.Id, res.Skip(3).First().Sys.Id);
        }

        [Fact]
        public async Task ComplexStructureWithSelfReferenceInArrayIsDeserializedCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithSelfreference.json");

            //Act
            _client.ResolveEntriesSelectively = true;
            var res = await _client.GetEntries<SelfReferencerInArray>();
            _client.ResolveEntriesSelectively = false;

            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal(res.First().SubCategories.First().Sys.Id, res.Skip(3).First().Sys.Id);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForSingleAsset()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"SampleAsset.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetAsset("434");

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/assets/434", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForAssets()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"AssetsCollection.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetAssets();

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/assets/", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForSingleEntry()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"SampleEntry.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetEntry<dynamic>("444");

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/entries/444", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForEntries()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"EntriesCollection.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetEntries<dynamic>();

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/entries", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForSingleContentType()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetContentType("123");

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/content_types/123", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForContentTypes()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"ContenttypesCollection.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };
            //Act
            await client.GetContentTypes();

            //Assert
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/content_types/", path);
        }

        [Fact]
        public async Task SettingEnvironmentShouldYieldCorrectUrlForSyncInitial()
        {
            //Arrange
            var client = GetClientWithEnvironment();
            _handler.Response = GetResponseFromFile(@"InitialSyncNoNextPage.json");
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.SyncInitial();

            //Assert	
            Assert.Equal("https://cdn.contentful.com/spaces/564/environments/special/sync?initial=true", path);
        }

        [Fact]
        public async Task GetEntriesShouldSerializeCorrectlyWithRichTextField()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithRichTextField.json");
            _client.ContentTypeResolver = new RichTextResolver();
            //Act
            var res = await _client.GetEntries<RichTextModel>();

            //Assert
            Assert.NotNull(res.First().RichText);
        }

        [Fact]
        public async Task TurningRichTextIntoHtmlShouldYieldCorrectResult()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithRichTextField.json");
            _client.ContentTypeResolver = new RichTextResolver();

            var htmlrenderer = new HtmlRenderer();
            htmlrenderer.AddRenderer(new RichTextContentRenderer() { Order = 10 });
            htmlrenderer.AddRenderer(new RichTextContentRendererLinks() { Order = 10 });
            //Act
            var res = await _client.GetEntries<RichTextModel>();
            var html = await htmlrenderer.ToHtml(res.Skip(1).First().RichText);
            //Assert
            Assert.Contains("<h1>Heading 1</h1>", html); 
            Assert.Contains("<h2>Heading 2</h2>", html);
            Assert.Contains("<a href=\"Entry Hyperlink\">Embedded 1</a>", html);
            Assert.Contains("<div><h2>Embedded 2</h2></div>", html);
            Assert.Contains("<ul><li><p>Unordered List</p></li>", html);
            Assert.Contains("<ol><li><p>With Nested Ordered</p></li>", html);
            Assert.Contains("<blockquote><p>A quote</p></blockquote>", html);
            Assert.Contains("<a href=\"//images.ctfassets.net/jd7yc4wnatx3/4j1JCmvF6MqQymaMqi6OSu/d0013ff28dd2db0371315ea7e63a6e0b/cat.jpg\">Asset Hyp<em>erlink</em></a>", html);
            Assert.Contains("<hr>", html);
            Assert.Contains("<img src=\"//images.ctfassets.net/jd7yc4wnatx3/4j1JCmvF6MqQymaMqi6OSu/d0013ff28dd2db0371315ea7e63a6e0b/cat.jpg\" alt=\"cat\" />", html);
            Assert.Contains("<p><strong>Bold Text</strong></p><p><em>Italic Text</em></p><p><u>Underline Text</u></p>", html);
        }

        [Fact]
        public async Task TurningRichTextContentIntoHtmlShouldYieldCorrectResultWithSelectiveResolving()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionWithRichTextField.json");
            _client.ContentTypeResolver = new RichTextResolver();
            _client.ResolveEntriesSelectively = true;
            var htmlrenderer = new HtmlRenderer();
            htmlrenderer.AddRenderer(new RichTextContentRenderer() { Order = 10 });
            //Act
            var res = await _client.GetEntries<RichTextModel>();
            var html = await htmlrenderer.ToHtml(res.Skip(1).First().RichText);
            //Assert
            Assert.Contains("<h1>Heading 1</h1>", html);
            Assert.Contains("<h2>Heading 2</h2>", html);
            Assert.Contains("<div><h2>Embedded 2</h2></div>", html);
            Assert.Contains("<ul><li><p>Unordered List</p></li>", html);
            Assert.Contains("<ol><li><p>With Nested Ordered</p></li>", html);
            Assert.Contains("<blockquote><p>A quote</p></blockquote>", html);
            Assert.Contains("<a href=\"//images.ctfassets.net/jd7yc4wnatx3/4j1JCmvF6MqQymaMqi6OSu/d0013ff28dd2db0371315ea7e63a6e0b/cat.jpg\">Asset Hyp<em>erlink</em></a>", html);
            Assert.Contains("<hr>", html);
            Assert.Contains("<img src=\"//images.ctfassets.net/jd7yc4wnatx3/4j1JCmvF6MqQymaMqi6OSu/d0013ff28dd2db0371315ea7e63a6e0b/cat.jpg\" alt=\"cat\" />", html);
            Assert.Contains("<p><strong>Bold Text</strong></p><p><em>Italic Text</em></p><p><u>Underline Text</u></p>", html);
        }

        private ContentfulClient GetClientWithEnvironment(string env = "special")
        {
            var httpClient = new HttpClient(_handler);
            var options = new ContentfulOptions
            {
                DeliveryApiKey = "123",
                PreviewApiKey = "3123",
                Environment = env,
                SpaceId = "564"
            };
            var client = new ContentfulClient(httpClient, options);
            return client;
        }

        public class RichTextContentRenderer : IContentRenderer
        {
            public int Order { get; set; }

            public bool SupportsContent(IContent content)
            {
                return content is EntryStructure && (content as EntryStructure).Data.Target is RichTextModel && (content as EntryStructure).NodeType == "embedded-entry-block";
            } 

            public string Render(IContent content)
            {
                var model = (content as EntryStructure).Data.Target as RichTextModel;

                var sb = new StringBuilder();

                sb.Append("<div>");

                sb.Append($"<h2>{model.Body}</h2>");

                sb.Append("</div>");

                return sb.ToString();
            }

            public Task<string> RenderAsync(IContent content)
            {
                return Task.FromResult(Render(content));
            }
        }

        public class RichTextContentRendererLinks : IContentRenderer
        {
            public int Order { get; set; }

            public bool SupportsContent(IContent content)
            {
                return content is EntryStructure && 
                    (content as EntryStructure).Data.Target is RichTextModel && 
                    (content as EntryStructure).NodeType == "entry-hyperlink";
            }

            public string Render(IContent content)
            {
                var link = (content as EntryStructure);
                var model = (content as EntryStructure).Data.Target as RichTextModel;

                var sb = new StringBuilder();

                sb.Append($"<a href=\"{(link.Content.FirstOrDefault() as Text).Value}\">{model.Body}</a>");

                return sb.ToString();
            }

            public Task<string> RenderAsync(IContent content)
            {
                return Task.FromResult(Render(content));
            }
        }
    }
}
