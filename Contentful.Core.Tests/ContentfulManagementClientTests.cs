using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Contentful.Core.Tests
{
    public class ContentfulManagementClientTests : ClientTestsBase
    {
        private ContentfulManagementClient _client;
        private FakeMessageHandler _handler;
        private HttpClient _httpClient;
        public ContentfulManagementClientTests()
        {
            _handler = new FakeMessageHandler();
            _httpClient = new HttpClient(_handler);
            _client = new ContentfulManagementClient(_httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "564",
                SpaceId = "666",
                UsePreviewApi = false
            });
        }

        [Fact]
        public async Task CreatingManagementClientAndMakingCallShouldSetHeadersCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var userAgent = "";
            var authHeader = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                userAgent = request.Headers.GetValues("X-Contentful-User-Agent").First();
                authHeader = request.Headers.GetValues("Authorization").First();
            };
            var version = typeof(ContentfulClientBase).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

            //Act
            await _client.GetAsset("sdf");

            //Assert
            Assert.Equal("Bearer 564", authHeader);
            Assert.StartsWith($"sdk contentful.csharp/{version}", userAgent);
        }

        [Fact]
        public async Task CreateContentTypeShouldSerializeRequestCorrectly()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage()
            {
                Content = new StringContent("{}")
            };
            var contentType = new ContentType()
            {
                SystemProperties = new SystemProperties()
            };
            contentType.SystemProperties.Id = "123";
            contentType.Name = "Name";
            contentType.Fields = new List<Field>()
            {
                new Field()
                {
                    Name = "Field1",
                    Id = "field1",
                    @Type = "Symbol",
                    Required = true,
                    Localized = false,
                    Disabled = false,
                    Omitted = false
                },
                new Field()
                {
                    Name = "Field2",
                    Id = "field2",
                    @Type = "Location",
                    Required = false,
                    Localized = true,
                    Disabled = false,
                    Omitted = false
                },
                new Field()
                {
                    Name = "Field3",
                    Id = "field3",
                    @Type = "Text",
                    Required = false,
                    Localized = false,
                    Disabled = true,
                    Omitted = false,
                    Validations = new List<IFieldValidator>()
                    {
                        new SizeValidator(3,100)
                    }
                },
                new Field()
                {
                    Name = "Field4",
                    Id = "field4",
                    @Type = "Link",
                    Required = false,
                    Localized = false,
                    Disabled = false,
                    Omitted = false,
                    LinkType = "Asset"
                }
            };


            //Act
            var res = await _client.CreateOrUpdateContentType(contentType);

            //Assert
            Assert.Null(res.Name);
        }

        [Fact]
        public async Task EditorInterfaceShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");

            //Act
            var res = await _client.GetEditorInterface("someid");

            //Assert
            Assert.Equal(7, res.Controls.Count);
            Assert.IsType<BooleanEditorInterfaceControlSettings>(res.Controls[4].Settings);
        }

        [Fact]
        public async Task CreateSpaceShouldCreateCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpace.json");
            var headerSet = false;
            var contentSet = "";
            _handler.VerificationBeforeSend = () =>
            {
            };
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                headerSet = request.Headers.GetValues("X-Contentful-Organization").First() == "112";
            };

            //Act
            var res = await _client.CreateSpace("Spaceman", "en-US", "112");

            //Assert
            Assert.True(headerSet);
            Assert.Equal(@"{""name"":""Spaceman"",""defaultLocale"":""en-US""}", contentSet);
            Assert.Equal("Products", res.Name);
        }

        [Fact]
        public async Task UpdateSpaceShouldCreateCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpace.json");
            var headerSet = false;
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                headerSet = request.Headers.GetValues("X-Contentful-Version").First() == "37";
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateSpaceName("spaceId", "Spacemaster", 37, "333");

            //Assert
            Assert.True(headerSet);
            Assert.Equal(@"{""name"":""Spacemaster""}", contentSet);
            Assert.Equal("Products", res.Name);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("54")]
        [InlineData("345")]
        public async Task DeletingASpaceShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteSpace(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/{id}", requestUrl);
        }

        [Fact]
        public async Task GetContentTypesShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            //Act
            var res = await _client.GetContentTypes();
            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);
        }

        [Fact]
        public async Task CreateOrUpdateContentTypeShouldThrowIfNoIdSet()
        {
            //Arrange
            var contentType = new ContentType()
            {
                Name = "Barbossa"
            };
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateContentType(contentType));

            //Assert
            Assert.Equal($"The id of the content type must be set.{Environment.NewLine}Parameter name: contentType", ex.Message);
        }

        [Fact]
        public async Task CreateOrUpdateContentTypeShouldCreateCorrectObject()
        {
            //Arrange
            var contentType = new ContentType()
            {
                Name = "Barbossa",
                SystemProperties = new SystemProperties()
            };
            contentType.SystemProperties.Id = "323";
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");

            var versionHeader = "";
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            var res = await _client.CreateOrUpdateContentType(contentType, version: 22);

            //Assert
            Assert.Equal("22", versionHeader);
            Assert.Contains(@"""name"":""Barbossa""", contentSet);
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
        public async Task GetContentTypeShouldThrowForEmptyId()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");

            //Act
            var res = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetContentType(""));

            //Assert
            Assert.Equal("contentTypeId", res.Message);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("456")]
        [InlineData("666")]
        public async Task DeletingAContentTypeShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteContentType(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/content_types/{id}", requestUrl);
        }

        [Fact]
        public async Task DeletingAContentTypeShouldThrowForEmptyId()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.DeleteContentType(""));

            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task ActivatingContentTypeShouldThrowForEmptyId()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.ActivateContentType("", 34));

            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task ActivatingContentTypeShouldCallCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");

            var versionHeader = "";
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            var res = await _client.ActivateContentType("758", 345);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/content_types/758/published", requestUrl);
            Assert.Equal("345", versionHeader);
        }

        [Fact]
        public async Task DeactivatingContentTypeShouldThrowForEmptyId()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.DeactivateContentType(""));

            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task DeactivatingContentTypeShouldCallCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");

            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;

            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeactivateContentType("324");

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/content_types/324/published", requestUrl);
        }

        [Fact]
        public async Task GetActivatedContentTypesShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            var requestUrl = "";

            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.GetActivatedContentTypes();

            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal($"https://api.contentful.com/spaces/666/public/content_types/", requestUrl);
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);
            Assert.Equal(8, ((res.First().Fields.Last().Validations.First() as NodesValidator).EmbeddedEntryInline.First() as SizeValidator).Max);
            Assert.Equal(2, ((res.First().Fields.Last().Validations.First() as NodesValidator).EmbeddedEntryBlock.First() as SizeValidator).Min);
            Assert.Equal(4, ((res.First().Fields.Last().Validations.First() as NodesValidator).EmbeddedEntryBlock.First() as SizeValidator).Max);
        }

        [Fact]
        public async Task GetEditorInterfaceShouldThrowForEmptyId()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEditorInterface(""));
            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task GetEditorInterfaceShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");

            //Act
            var res = await _client.GetEditorInterface("23");
            //Assert
            Assert.Equal(7, res.Controls.Count);
            Assert.IsType<BooleanEditorInterfaceControlSettings>(res.Controls[4].Settings);
            Assert.IsType<RatingEditorInterfaceControlSettings>(res.Controls[5].Settings);
            Assert.Equal(7, (res.Controls[5].Settings as RatingEditorInterfaceControlSettings).NumberOfStars);
            Assert.Equal("How many do you likez?", res.Controls[5].Settings.HelpText);
            Assert.IsType<DatePickerEditorInterfaceControlSettings>(res.Controls[6].Settings);
            Assert.Equal(EditorInterfaceDateFormat.time, (res.Controls[6].Settings as DatePickerEditorInterfaceControlSettings).DateFormat);

        }

        [Fact]
        public async Task UpdateEditorInterfaceShouldThrowForEmptyId()
        {
            //Arrange
            var editorInterface = new EditorInterface();
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.UpdateEditorInterface(editorInterface, "", 6));
            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task UpdateEditorInterfaceShouldCallCorrectUrl()
        {
            //Arrange
            var editorInterface = new EditorInterface()
            {
                Controls = new List<EditorInterfaceControl>()
            {
                new EditorInterfaceControl()
                {
                    FieldId = "field1",
                    WidgetId = SystemWidgetIds.SingleLine
                },
                new EditorInterfaceControl()
                {
                    FieldId = "field2",
                    WidgetId = SystemWidgetIds.Boolean,
                    Settings = new BooleanEditorInterfaceControlSettings()
                    {
                        HelpText = "Help me here!",
                        TrueLabel = "Truthy",
                        FalseLabel = "Falsy"
                    }
                }
            }
            };
            var versionHeader = "";
            var contentSet = "";
            var requestUrl = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");

            //Act
            var res = await _client.UpdateEditorInterface(editorInterface, "123", 16);

            //Assert
            Assert.Equal("16", versionHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/content_types/123/editor_interface", requestUrl);
            Assert.Equal(@"{""controls"":[{""fieldId"":""field1"",""widgetId"":""singleLine""},{""fieldId"":""field2"",""widgetId"":""boolean"",""settings"":{""trueLabel"":""Truthy"",""falseLabel"":""Falsy"",""helpText"":""Help me here!""}}]}", contentSet);
        }

        [Fact]
        public async Task EditorInterfaceShouldSerializeCorrectly()
        {
            //Arrange
            var editorInterface = new EditorInterface()
            {
                Controls = new List<EditorInterfaceControl>()
            {
                new EditorInterfaceControl()
                {
                    FieldId = "field1",
                    WidgetId = SystemWidgetIds.SingleLine
                },
                new EditorInterfaceControl()
                {
                    FieldId = "field2",
                    WidgetId = SystemWidgetIds.Boolean,
                    Settings = new BooleanEditorInterfaceControlSettings()
                    {
                        HelpText = "Help me here!",
                        TrueLabel = "Truthy",
                        FalseLabel = "Falsy"
                    }
                }
            }
            };
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");

            //Act
            var res = await _client.UpdateEditorInterface(editorInterface, "123", 1);

            //Assert
            Assert.Equal(7, res.Controls.Count);
            Assert.IsType<BooleanEditorInterfaceControlSettings>(res.Controls[4].Settings);
        }

        [Fact]
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionManagement.json");

            //Act
            var res = await _client.GetEntriesCollection<Entry<dynamic>>();

            //Assert
            Assert.Equal(8, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(8, res.Items.Count());
            Assert.Equal("Somethi", res.First().Fields.field1["en-US"].ToString());
            Assert.Equal(DateTime.Parse("2016-11-23T09:40:56.857Z").ToUniversalTime(), res.First().SystemProperties.CreatedAt);
            Assert.Equal("testagain", res.First().SystemProperties.ContentType.SystemProperties.Id);
            Assert.Equal(1, res.First().SystemProperties.PublishedCounter);
        }

        [Fact]
        public async Task GetEntriesForLocaleShouldSerializeIntoCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionManagement.json");

            //Act
            var res = await _client.GetEntriesForLocale<TestNested>(null, "en-US");

            //Assert
            Assert.Equal(8, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(8, res.Items.Count());
            Assert.Equal("Somethi", res.First().Field1);
        }

        [Fact]
        public async Task GetEntriesCollectionShouldSerializeIntoCorrectCollectionWithCustomTypes()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionManagement.json");

            //Act
            var res = await _client.GetEntriesCollection<ManagementEntry>();

            //Assert
            Assert.Equal(8, res.Total);
            Assert.Equal(100, res.Limit);
            Assert.Equal(0, res.Skip);
            Assert.Equal(8, res.Items.Count());
            Assert.Equal("Somethi", res.First().Field1["en-US"].ToString());
            Assert.Equal(DateTime.Parse("2016-11-23T09:40:56.857Z").ToUniversalTime(), res.First().Sys.CreatedAt);
            Assert.Equal("testagain", res.First().Sys.ContentType.SystemProperties.Id);
            Assert.Equal(1, res.First().Sys.PublishedCounter);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldThrowIfIdIsNotSet()
        {
            //Arrange
            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties()
            };
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateEntry(entry, contentTypeId: "Hwoarang"));
            //Assert
            Assert.Equal("The id of the entry must be set.", ex.Message);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldAddCorrectContentTypeHeader()
        {
            //Arrange
            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties()
            };
            entry.SystemProperties.Id = "123";
            var contentTypeHeader = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateOrUpdateEntry(entry, contentTypeId: "Eddie Gordo");
            //Assert
            Assert.Equal("Eddie Gordo", contentTypeHeader);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldNotAddContentTypeHeaderIfNotSet()
        {
            //Arrange
            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties()
            };
            entry.SystemProperties.Id = "123";
            IEnumerable<string> contentTypeHeader = new List<string>();
            _handler.VerificationBeforeSend = () =>
            {
                _httpClient.DefaultRequestHeaders.TryGetValues("X-Contentful-Content-Type", out contentTypeHeader);
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            //Act
            var res = await _client.CreateOrUpdateEntry(entry, version: 7);
            //Assert
            Assert.Null(contentTypeHeader);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldCallCorrectUrlWithData()
        {
            //Arrange
            var entry = new Entry<dynamic>()
            {
                SystemProperties = new SystemProperties()
            };
            entry.SystemProperties.Id = "532";
            entry.Fields = new ExpandoObject();
            entry.Fields.field34 = new Dictionary<string, string>()
            {
                { "en-US", "banana" }
            };
            var contentTypeHeader = "";
            var contentSet = "";
            var requestUrl = "";
            var versionHeader = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateOrUpdateEntry(entry, contentTypeId: "Bryan Fury", version: 45);
            //Assert
            Assert.Equal("Bryan Fury", contentTypeHeader);
            Assert.Equal("45", versionHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/entries/532", requestUrl);
            Assert.Contains(@"""field34"":{""en-US"":""banana""}", contentSet);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldCallCorrectUrlWithDataForCustomType()
        {
            //Arrange
            var entry = new ManagementEntry
            {
                Field34 = new Dictionary<string, string>()
            {
                { "en-US", "banana" }
            }
            };
            var contentTypeHeader = "";
            var contentSet = "";
            var requestUrl = "";
            var versionHeader = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateOrUpdateEntry(entry, "532", contentTypeId: "Bryan Fury", version: 45);
            //Assert
            Assert.Equal("Bryan Fury", contentTypeHeader);
            Assert.Equal("45", versionHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/entries/532", requestUrl);
            Assert.Contains(@"""field34"":{""en-US"":""banana""}", contentSet);
        }

        [Fact]
        public async Task CreateOrUpdateEntryShouldCallCorrectUrlWithDataForRichText()
        {
            //Arrange
            var doc = new Document()
            {
                NodeType = "document",
                Content = new List<IContent>()
                {
                    new Paragraph()
                    {
                        NodeType = "paragraph",
                        Content = new List<IContent>()
                        {
                            new Text()
                            {
                                Value = "Hello!"
                            }
                        }
                    }
                }
            };
            var entry = new ManagementEntry
            {
                Field34 = new Dictionary<string, string>()
                {
                    { "en-US", "banana" }
                },
                RichText = new Dictionary<string, Document>()
                {
                    { "en-US", doc }
                }
            };
            var contentTypeHeader = "";
            var contentSet = "";
            var requestUrl = "";
            var versionHeader = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateOrUpdateEntry(entry, "532", contentTypeId: "Bryan Fury", version: 45);
            //Assert
            Assert.Equal("Bryan Fury", contentTypeHeader);
            Assert.Equal("45", versionHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/entries/532", requestUrl);
            Assert.Contains(@"""field34"":{""en-US"":""banana""}", contentSet);
            Assert.Contains(@"""richText"":{""en-US"":{""nodeType"":""document"",""data"":null,""content"":[{""nodeType"":""paragraph"",""data"":null,""content"":[{""nodeType"":null,""data"":null,""value"":""Hello!"",""marks"":null}]}", contentSet);
        }

        [Fact]
        public async Task UpdateEntryForLocaleShouldSetValuesCorrectly()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Benko"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleContentTypeFields.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentSet = "";

            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateEntryForLocale(entry, "532", locale: "en-US");
            //Assert

            Assert.Contains(@"""field1"":{""en-US"":""Benko""}", contentSet);
        }

        [Fact]
        public async Task CreateEntryForLocaleShouldSetValuesCorrectly()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Fischer"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentSet = "";

            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateEntryForLocale(entry, "532", "contentType", locale: "sv-SE");
            //Assert

            Assert.Contains(@"""field1"":{""sv-SE"":""Fischer""}", contentSet);
        }

        [Fact]
        public async Task CreateEntryForLocaleShouldSetCorrectContentTypeHeader()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Fischer"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentTypeHeader = "";

            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
            };


            //Act
            var res = await _client.CreateEntryForLocale(entry, "532", "contentType", locale: "sv-SE");
            //Assert

            Assert.Contains("contentType", contentTypeHeader);
        }

        [Fact]
        public async Task CreateEntryForLocaleShouldSetValuesCorrectlyWhenNoLocaleIsSpecified()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Alekhine"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"LocalesCollection.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentSet = "";

            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateEntryForLocale(entry, "532", "contentType");
            //Assert

            Assert.Contains(@"""field1"":{""en-US"":""Alekhine""}", contentSet);
        }

        [Fact]
        public async Task UpdateEntryForLocaleShouldSetValuesCorrectlyWhenNoLocaleIsSpecified()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Benko"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleContentTypeFields.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"LocalesCollection.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentSet = "";

            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateEntryForLocale(entry, "532");
            //Assert

            Assert.Contains(@"""field1"":{""en-US"":""Benko""}", contentSet);
        }

        [Fact]
        public async Task UpdateEntryForLocaleShouldSetValuesCorrectlyWithFieldsNotPresentInEntry()
        {
            //Arrange
            var entry = new TestNested
            {
                Field1 = "Benko",
                NewField = "This is new!"
            };
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleContentTypeFields.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"LocalesCollection.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleEntryManagement.json"));
            var contentSet = "";

            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateEntryForLocale(entry, "532");
            //Assert

            Assert.Contains(@"""field1"":{""en-US"":""Benko""}", contentSet);
            Assert.Contains(@"""newField"":{""en-US"":""This is new!""}", contentSet);
        }

        [Fact]
        public async Task CreateEntryShouldCallCorrectUrlWithData()
        {
            //Arrange
            var entry = new Entry<dynamic>
            {
                Fields = new ExpandoObject()
            };
            entry.Fields.field34 = new Dictionary<string, string>()
            {
                { "en-US", "bapple" }
            };
            var contentTypeHeader = "";
            var contentSet = "";
            var requestUrl = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateEntry(entry, contentTypeId: "Ling Xiaoyu");
            //Assert
            Assert.Equal("Ling Xiaoyu", contentTypeHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/entries", requestUrl);
            Assert.Contains(@"""field34"":{""en-US"":""bapple""}", contentSet);
        }

        [Fact]
        public async Task CreateEntryWithCustomTypeShouldCallCorrectUrlWithData()
        {
            //Arrange
            var entry = new ManagementEntry()
            {
                Field34 = new Dictionary<string, string>()
                {
                    { "en-US", "bapple" }
                }
            };
            var contentTypeHeader = "";
            var contentSet = "";
            var requestUrl = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                contentTypeHeader = request.Headers.GetValues("X-Contentful-Content-Type").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.CreateEntry(entry, contentTypeId: "Ling Xiaoyu");
            //Assert
            Assert.Equal("Ling Xiaoyu", contentTypeHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/entries", requestUrl);
            Assert.Contains(@"""field34"":{""en-US"":""bapple""}", contentSet);
        }

        [Fact]
        public async Task CreateEntryShouldThrowIfContentTypeIsNotSet()
        {
            //Arrange
            var entry = new Entry<dynamic>();

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateEntry(entry, contentTypeId: ""));
            //Assert
            Assert.Equal($"The content type id must be set.{Environment.NewLine}Parameter name: contentTypeId", ex.Message);
        }

        [Fact]
        public async Task GetEntryShouldThrowIfIdIsNotSet()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEntry(""));
            //Assert
            Assert.Equal("entryId", ex.Message);
        }

        [Fact]
        public async Task GetEntryShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            var res = await _client.GetEntry("43");
            //Assert
            Assert.Equal("42BwcCt4TeeskG0eq8S2CQ", res.SystemProperties.Id);
            Assert.Equal("Somethi", res.Fields.field1["en-US"].ToString());
        }

        [Theory]
        [InlineData("777")]
        [InlineData("456")]
        [InlineData("666")]
        public async Task DeletingAnEntryShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.DeleteEntry(id, 32);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal("32", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/entries/{id}", requestUrl);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("abc")]
        [InlineData("666")]
        public async Task PublishingAnEntryShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            await _client.PublishEntry(id, 23);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("23", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/entries/{id}/published", requestUrl);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("abc")]
        [InlineData("666")]
        public async Task UnpublishingAnEntryShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            await _client.PublishEntry(id, 23);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("23", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/entries/{id}/published", requestUrl);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("abc")]
        [InlineData("666")]
        public async Task ArchivingAnEntryShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            await _client.ArchiveEntry(id, 78);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("78", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/entries/{id}/archived", requestUrl);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("abc")]
        [InlineData("666")]
        public async Task UnarchivingAnEntryShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");

            //Act
            await _client.UnarchiveEntry(id, 67);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal("67", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/entries/{id}/archived", requestUrl);
        }

        [Fact]
        public async Task GetAllAssetsShouldReturnCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollectionManagement.json");

            //Act
            var res = await _client.GetAssetsCollection();

            //Assert
            Assert.Equal(7, res.Count());
            Assert.Equal(7, res.Total);
            Assert.Equal("Ernest Hemingway (1950)", res.First().Title["en-US"]);
            Assert.Equal(2290561, res.First().Files["en-US"].Details.Size);
            Assert.Equal(2940, res.First().Files["en-US"].Details.Image.Width);
            Assert.Equal("Alice in Wonderland", res.Last().Title["en-US"]);
        }

        [Theory]
        [InlineData("?fields.title=bam")]
        [InlineData("?fields.description[exists]=false")]
        [InlineData("?order=sys.createdAt")]
        public async Task GetAllAssetsWithQueryShouldCallCorrectUrl(string query)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollectionManagement.json");

            var requestUrl = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.GetAssetsCollection(query);

            //Assert
            Assert.Equal(7, res.Count());
            Assert.Equal(7, res.Total);
            Assert.Equal("Ernest Hemingway (1950)", res.First().Title["en-US"]);
            Assert.Equal(2290561, res.First().Files["en-US"].Details.Size);
            Assert.Equal(2940, res.First().Files["en-US"].Details.Image.Width);
            Assert.Equal("Alice in Wonderland", res.Last().Title["en-US"]);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{query}", requestUrl);
        }

        [Fact]
        public async Task CreateAssetShouldThrowIfIdNotSet()
        {
            //Arrange
            var asset = new ManagementAsset()
            {
                Title = new Dictionary<string, string>()
            };
            asset.Title["en-US"] = "Burton Green";

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateAsset(asset));
            //Assert
            Assert.Equal("The id of the asset must be set.", ex.Message);
        }

        [Fact]
        public async Task CreateOrUpdateAssetShouldCallCorrectUrlWithCorrectData()
        {
            //Arrange
            var asset = new ManagementAsset()
            {
                Title = new Dictionary<string, string>()
            };
            asset.Title["en-US"] = "Burton Green";
            asset.SystemProperties = new SystemProperties()
            {
                Id = "424"
            };
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var contentSet = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.CreateOrUpdateAsset(asset);
            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Contains(@"""en-US"":""Burton Green""", contentSet);
            Assert.Equal("https://api.contentful.com/spaces/666/assets/424", requestUrl);
        }

        [Fact]
        public async Task CreateAssetShouldCallCorrectUrlWithCorrectData()
        {
            //Arrange
            var asset = new ManagementAsset()
            {
                Title = new Dictionary<string, string>()
            };
            asset.Title["en-US"] = "Burton Green";
            asset.SystemProperties = new SystemProperties()
            {
                Id = "424"
            };
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var contentSet = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.CreateAsset(asset);
            //Assert
            Assert.Equal(HttpMethod.Post, requestMethod);
            Assert.Contains(@"""en-US"":""Burton Green""", contentSet);
            Assert.Equal("https://api.contentful.com/spaces/666/assets", requestUrl);
        }

        [Fact]
        public async Task CreateAssetShouldSetVersionHeaderCorrectly()
        {
            //Arrange
            var asset = new ManagementAsset()
            {
                Title = new Dictionary<string, string>()
            };
            asset.Title["en-US"] = "Burton Green";
            asset.SystemProperties = new SystemProperties()
            {
                Id = "424"
            };
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var versionHeader = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            var res = await _client.CreateOrUpdateAsset(asset, version: 234);
            //Assert
            Assert.Equal("234", versionHeader);
        }

        [Fact]
        public async Task GetAllPublishedAssetsShouldReturnCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollectionManagement.json");

            //Act
            var res = await _client.GetPublishedAssetsCollection();

            //Assert
            Assert.Equal(7, res.Count());
            Assert.Equal(7, res.Total);
            Assert.Equal("Ernest Hemingway (1950)", res.First().Title["en-US"]);
            Assert.Equal(2290561, res.First().Files["en-US"].Details.Size);
            Assert.Equal(2940, res.First().Files["en-US"].Details.Image.Width);
            Assert.Equal("Alice in Wonderland", res.Last().Title["en-US"]);
        }

        [Fact]
        public async Task GetAssetShouldThrowIfNoIdSet()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetAsset(""));

            //Assert
            Assert.Equal("assetId", ex.Message);
        }

        [Fact]
        public async Task GetAssetShouldReturnCorrectItem()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");

            //Act
            var res = await _client.GetAsset("234");

            //Assert
            Assert.Equal("Ernest Hemingway (1950)", res.Title["en-US"]);
            Assert.Equal("Hemingway in the cabin of his boat Pilar, off the coast of Cuba", res.Description["en-US"]);
        }

        [Theory]
        [InlineData("7HG7")]
        [InlineData("asf")]
        [InlineData("666")]
        public async Task DeletingAnAssetShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.DeleteAsset(id, 86);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal("86", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}", requestUrl);
        }

        [Theory]
        [InlineData("7HG7", "en-US")]
        [InlineData("asf", "sv")]
        [InlineData("g3445g", "cucumber")]
        public async Task ProcessingAnAssetShouldCallCorrectUrl(string id, string locale)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.ProcessAsset(id, 25, locale);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("25", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}/files/{locale}/process", requestUrl);
        }

        [Fact]
        public async Task ProcessingAsseToCompletionShouldReturnCorrectAsset()
        {
            //Arrange
            _handler.Responses.Enqueue(new HttpResponseMessage());
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagementUnprocessed.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagementUnprocessed.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagement.json"));

            //Act
            var res = await _client.ProcessAssetUntilCompleted("123", 3, "en-US");

            //Assert
            Assert.NotNull(res);
            Assert.Equal("Ernest Hemingway (1950)", res.Title["en-US"]);
            Assert.Equal("Hemingway in the cabin of his boat Pilar, off the coast of Cuba", res.Description["en-US"]);
        }

        [Fact]
        public async Task ProcessingAssetToCompletionShouldFailAfterTimeout()
        {
            //Arrange
            _handler.Responses.Enqueue(new HttpResponseMessage());
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagementUnprocessed.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagementUnprocessed.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagement.json"));
            //Act
            var ex = await Assert.ThrowsAsync<TimeoutException>(async () => await _client.ProcessAssetUntilCompleted("123", 3, "en-US", 300));

            //Assert
            Assert.Equal("The processing of the asset did not finish in a timely manner. Max delay of 300 reached.", ex.Message);
        }

        [Theory]
        [InlineData("7HG7")]
        [InlineData("asf")]
        [InlineData("666")]
        public async Task PublishingAnAssetShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.PublishAsset(id, 23);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("23", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}/published", requestUrl);
        }

        [Theory]
        [InlineData("7HG7")]
        [InlineData("ab45")]
        [InlineData("666")]
        public async Task UnpublishingAnAssetShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.UnpublishAsset(id, 38);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal("38", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}/published", requestUrl);
        }

        [Theory]
        [InlineData("asg")]
        [InlineData("435")]
        [InlineData("785685fgh")]
        public async Task ArchivingAnAssetShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.ArchiveAsset(id, 23);

            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("23", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}/archived", requestUrl);
        }

        [Theory]
        [InlineData("57y43hr")]
        [InlineData("346w")]
        [InlineData("babt3")]
        public async Task UnarchivingAnAssetShouldCallCorrectUrl(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var requestUrl = "";
            var versionHeader = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
                versionHeader = request.Headers.GetValues("X-Contentful-Version").First();
            };

            //Act
            await _client.UnarchiveAsset(id, 89);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal("89", versionHeader);
            Assert.Equal($"https://api.contentful.com/spaces/666/assets/{id}/archived", requestUrl);
        }

        [Fact]
        public async Task GetAllLocalesShouldReturnCorrectCollection()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"LocalesCollection.json");

            //Act
            var res = await _client.GetLocalesCollection();

            //Assert
            Assert.Equal(1, res.Count());
            Assert.Equal(1, res.Total);
            Assert.Equal("U.S. English", res.First().Name);
            Assert.Equal("en-US", res.First().Code);
            Assert.True(res.First().Default);
            Assert.Null(res.First().FallbackCode);
        }

        [Fact]
        public async Task CreateLocaleShouldCreateCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");
            var locale = new Locale()
            {
                Name = "Unintelligible English",
                FallbackCode = "sv-SE",
                Optional = true,
                Code = "en-UI"
            };
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateLocale(locale);

            //Assert
            Assert.Equal(@"{""code"":""en-UI"",""contentDeliveryApi"":false,""contentManagementApi"":false,""fallbackCode"":""sv-SE"",""name"":""Unintelligible English"",""optional"":true}", contentSet);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetLocaleShouldThrowWhenIdIsNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetLocale(id));

            //Assert
            Assert.Equal("The localeId must be set.", ex.Message);
        }

        [Fact]
        public async Task GetLocaleShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");

            //Act
            var res = await _client.GetLocale("art");

            //Assert
            Assert.Equal("U.S. English", res.Name);
            Assert.Equal("en-US", res.Code);
        }

        [Theory]
        [InlineData("asg")]
        [InlineData("21345")]
        [InlineData("hgf633f")]
        public async Task DeletingLocaleShouldCallCorrectUrl(string id)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };


            //Act
            await _client.DeleteLocale(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/locales/{id}", requestUrl);
        }

        [Fact]
        public async Task GetAllWebhooksShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCollection.json");

            //Act
            var res = await _client.GetWebhooksCollection();

            //Assert
            Assert.Equal(1, res.Total);
            Assert.Equal(1, res.Count());
            Assert.Equal("Testhook", res.First().Name);
            Assert.Equal(2, res.First().Filters.Count);
            Assert.IsType(typeof(EqualsConstraint), res.First().Filters.First());
            Assert.IsType(typeof(InConstraint), (res.First().Filters.Last() as NotConstraint).ConstraintToInvert);
            Assert.Equal("https://robertlinde.se/", res.First().Url);
        }

        [Fact]
        public async Task GetAllWebhooksShouldDeserializeCorrectlyWithQueryString()
        {
            //Arrange
            var url = "";
            _handler.Response = GetResponseFromFile(@"WebhookCollection.json");
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                url = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.GetWebhooksCollection(queryString: "?skip=3&take=7");

            //Assert
            Assert.Equal(1, res.Total);
            Assert.Equal(1, res.Count());
            Assert.Equal("https://api.contentful.com/spaces/666/webhook_definitions?skip=3&take=7", url);
            Assert.Equal("Testhook", res.First().Name);
            Assert.Equal(2, res.First().Filters.Count);
            Assert.IsType(typeof(EqualsConstraint), res.First().Filters.First());
            Assert.IsType(typeof(InConstraint), (res.First().Filters.Last() as NotConstraint).ConstraintToInvert);
            Assert.Equal("https://robertlinde.se/", res.First().Url);
        }

        [Fact]
        public async Task CreateWebhookShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleWebHook.json");

            var webhook = new Webhook()
            {
                Name = "Some hook",
                Url = "https://www.cracked.com/",
                HttpBasicPassword = "Tepes",
                HttpBasicUsername = "Vlad",
                Topics = new List<string>()
            {
                "Entry.create",
                "Asset.publish"
            }
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateWebhook(webhook);

            //Assert
            Assert.Equal(HttpMethod.Post, method);
            Assert.Equal("https://api.contentful.com/spaces/666/webhook_definitions", url);
            Assert.Contains(@"""name"":""Some hook""", contentSet);
            Assert.Contains(@"""url"":""https://www.cracked.com/""", contentSet);
            Assert.Contains(@"""httpBasicUsername"":""Vlad""", contentSet);
            Assert.Contains(@"""httpBasicPassword"":""Tepes""", contentSet);
            Assert.Contains(@"""Entry.create""", contentSet);
            Assert.Contains(@"""Asset.publish""", contentSet);
            Assert.DoesNotContain(@"""transformation""", contentSet);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateOrUpdateWebhookShouldThrowIfNoIdSet(string id)
        {
            //Arrange
            var webHook = new Webhook()
            {
                SystemProperties = new SystemProperties()
            };
            webHook.SystemProperties.Id = id;

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateWebhook(webHook));

            //Assert
            Assert.Equal("The id of the webhook must be set.", ex.Message);
        }

        [Theory]
        [InlineData("2354")]
        [InlineData("33")]
        [InlineData("vadfb#¤123")]
        public async Task CreateOrUpdateWebhookShouldCallCorrectUrlWithData(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleWebHook.json");

            var webhook = new Webhook()
            {
                SystemProperties = new SystemProperties()
            };
            webhook.SystemProperties.Id = id;
            webhook.Name = "Bingo";
            webhook.Url = "http://www.imdb.com/name/nm0001159/";
            webhook.HttpBasicPassword = "Caligula";
            webhook.HttpBasicUsername = "Emperor";
            webhook.Topics = new List<string>()
            {
                "Asset.create",
                "Entry.*"
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrUpdateWebhook(webhook);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/spaces/666/webhook_definitions/{id}", url);
            Assert.Contains(@"""name"":""Bingo""", contentSet);
            Assert.Contains(@"""url"":""http://www.imdb.com/name/nm0001159/""", contentSet);
            Assert.Contains(@"""httpBasicUsername"":""Emperor""", contentSet);
            Assert.Contains(@"""httpBasicPassword"":""Caligula""", contentSet);
            Assert.Contains(@"""Asset.create""", contentSet);
            Assert.Contains(@"""Entry.*""", contentSet);
            Assert.DoesNotContain(@"""transformation""", contentSet);
        }

        [Theory]
        [InlineData("654")]
        [InlineData("123")]
        [InlineData("bill")]
        public async Task CreateOrUpdateWebhookShouldCallCorrectUrlWithDataAndTransformation(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleWebHook.json");

            var webhook = new Webhook()
            {
                SystemProperties = new SystemProperties()
            };
            webhook.SystemProperties.Id = id;
            webhook.Name = "Canabanana";
            webhook.Url = "http://www.imdb.com/name/nm0000549/";
            webhook.HttpBasicPassword = "Roger";
            webhook.HttpBasicUsername = "Wilco";
            webhook.Topics = new List<string>()
            {
                "Asset.create",
                "Entry.*"
            };

            var transform = new WebhookTransformation
            {
                Body = new { SomeStuff = "hello!" },
                ContentType = TransformationContentTypes.ContentfulManagementPlusJsonAndCharset,
                Method = HttpMethods.PUT
            };

            webhook.Transformation = transform;

            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrUpdateWebhook(webhook);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/spaces/666/webhook_definitions/{id}", url);
            Assert.Contains(@"""name"":""Canabanana""", contentSet);
            Assert.Contains(@"""url"":""http://www.imdb.com/name/nm0000549/""", contentSet);
            Assert.Contains(@"""httpBasicUsername"":""Wilco""", contentSet);
            Assert.Contains(@"""httpBasicPassword"":""Roger""", contentSet);
            Assert.Contains(@"""Asset.create""", contentSet);
            Assert.Contains(@"""Entry.*""", contentSet);
            Assert.Contains(@"""transformation""", contentSet);
            Assert.Contains(@"""contentType""", contentSet);
            Assert.Contains(@"""application/vnd.contentful.management.v1+json; charset=utf-8""", contentSet);
        }

        [Theory]
        [InlineData("654")]
        [InlineData("123")]
        [InlineData("bill")]
        public async Task CreateOrUpdateWebhookShouldCallCorrectUrlWithDataAndTransformationWithNullBody(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleWebHook.json");

            var webhook = new Webhook()
            {
                SystemProperties = new SystemProperties()
            };
            webhook.SystemProperties.Id = id;
            webhook.Name = "Canabanana";
            webhook.Url = "http://www.imdb.com/name/nm0000549/";
            webhook.HttpBasicPassword = "Roger";
            webhook.HttpBasicUsername = "Wilco";
            webhook.Topics = new List<string>()
            {
                "Asset.create",
                "Entry.*"
            };

            var transform = new WebhookTransformation
            {
                ContentType = TransformationContentTypes.ContentfulManagementPlusJsonAndCharset,
                Method = HttpMethods.PUT
            };

            webhook.Transformation = transform;

            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrUpdateWebhook(webhook);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/spaces/666/webhook_definitions/{id}", url);
            Assert.Contains(@"""name"":""Canabanana""", contentSet);
            Assert.Contains(@"""url"":""http://www.imdb.com/name/nm0000549/""", contentSet);
            Assert.Contains(@"""httpBasicUsername"":""Wilco""", contentSet);
            Assert.Contains(@"""httpBasicPassword"":""Roger""", contentSet);
            Assert.Contains(@"""Asset.create""", contentSet);
            Assert.Contains(@"""Entry.*""", contentSet);
            Assert.Contains(@"""transformation""", contentSet);
            Assert.Contains(@"""contentType""", contentSet);
            Assert.Contains(@"""application/vnd.contentful.management.v1+json; charset=utf-8""", contentSet);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetWebhookShouldThrowIfNoIdSet(string id)
        {
            //Arrange


            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetWebhook(id));

            //Assert
            Assert.Equal($"The id of the webhook must be set.{Environment.NewLine}Parameter name: webhookId", ex.Message);
        }

        [Fact]
        public async Task GetWebhookShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleWebHook.json");

            //Act
            var res = await _client.GetWebhook("ertg");

            //Assert
            Assert.Equal("pops", res.Name);
            Assert.Equal("https://robertlinde.se", res.Url);
            Assert.Collection(res.Topics,
                (t) => Assert.Equal("ContentType.*", t),
                (t) => Assert.Equal("Entry.*", t));

            Assert.Collection(res.Headers, (h) => { 
                Assert.Equal("pip", h.Key); Assert.Null(h.Value); Assert.True(h.Secret); 
                },
                (h) => {
                    Assert.Equal("cus", h.Key); Assert.Equal("nobo", h.Value); Assert.Null(h.Secret);
                }
            );
        }

        [Theory]
        [InlineData("346")]
        [InlineData("yw345")]
        [InlineData("xcb345af")]
        public async Task DeletingWebhookShouldCallCorrectUrl(string id)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteWebhook(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/webhook_definitions/{id}", requestUrl);
        }

        [Fact]
        public async Task WebhookCallDetailsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCallDetails.json");
            //Act
            var res = await _client.GetWebhookCallDetails("b", "s");

            //Assert
            Assert.Equal("unarchive", res.EventType);
            Assert.Equal("close", res.Response.Headers["connection"]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WebhookCallDetailShouldThrowForWebHookIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCallDetails.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetWebhookCallDetails("some", id));

            //Assert
            Assert.Equal($"The id of the webhook must be set.{Environment.NewLine}Parameter name: webhookId", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WebhookCallDetailShouldThrowForWebHookCallIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCallDetails.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetWebhookCallDetails(id, "some"));

            //Assert
            Assert.Equal($"The id of the webhook call must be set.{Environment.NewLine}Parameter name: callId", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WebhookCallDetailsShouldThrowForIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCallDetailsCollection.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetWebhookCallDetailsCollection(id));

            //Assert
            Assert.Equal($"The id of the webhook must be set.{Environment.NewLine}Parameter name: webhookId", ex.Message);
        }

        [Fact]
        public async Task WebhookCallDetailsCollectionShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookCallDetailsCollection.json");

            //Act
            var res = await _client.GetWebhookCallDetailsCollection("aaf");

            //Assert
            Assert.Equal(2, res.Total);
            Assert.Equal(2, res.Count());
            Assert.Equal(403, res.First().StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WebhookHealthShouldThrowForIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookHealth.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetWebhookHealth(id));

            //Assert
            Assert.Equal($"The id of the webhook must be set.{Environment.NewLine}Parameter name: webhookId", ex.Message);
        }

        [Fact]
        public async Task WebhookHealthShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"WebhookHealth.json");

            //Act
            var res = await _client.GetWebhookHealth("aaf");

            //Assert
            Assert.Equal(2, res.TotalCalls);
            Assert.Equal(0, res.TotalHealthy);
        }

        [Fact]
        public void ConstraintsShouldSerializeCorrectly()
        {
            //Arrange
            var policy = new Policy()
            {
                Effect = "allow",
                Actions = new List<string>()
            {
                "create",
                "delete"
            },
                Constraint =
                new AndConstraint()
                {
                    new EqualsConstraint()
                    {
                        Property = "sys.type",
                        ValueToEqual = "Entry"
                    },
                    new EqualsConstraint()
                    {
                        Property = "sys.createdBy.sys.id",
                        ValueToEqual = "User.current()"
                    },
                    new NotConstraint()
                    {
                        ConstraintToInvert = new EqualsConstraint()
                        {
                            Property = "sys.contentType.sys.id",
                            ValueToEqual = "123"
                        }
                    }
            }
            };

            //Act
            var output = JsonConvert.SerializeObject(policy, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            //Assert
            Assert.Equal(@"{""effect"":""allow"",""actions"":[""create"",""delete""],""constraint"":{""and"":[{""equals"":[{""doc"":""sys.type""},""Entry""]},{""equals"":[{""doc"":""sys.createdBy.sys.id""},""User.current()""]},{""not"":{""equals"":[{""doc"":""sys.contentType.sys.id""},""123""]}}]}}", output);
        }

        [Fact]
        public async Task RoleShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleRole.json");

            //Act
            var res = await _client.GetRole("123");

            //Assert
            Assert.Equal("Developer", res.Name);
            Assert.Equal("sys.type", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).Property);
            Assert.Equal("Asset", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).ValueToEqual);
        }

        [Fact]
        public async Task RoleShouldSerializeCorrectly()
        {
            //Arrange
            var role = new Role();
            _handler.Response = GetResponseFromFile(@"SampleRole.json");


            role.Name = "test";
            role.Description = "desc";
            role.Permissions = new ContentfulPermissions()
            {
                ContentDelivery = new List<string>() { "all" },
                ContentModel = new List<string>() { "read" },
                Settings = new List<string>() { "read", "manage" }
            };
            role.Policies = new List<Policy>
            {
                new Policy()
                {
                    Effect = "allow",
                    Actions = new List<string>()
                {
                    "read",
                    "create",
                    "update"
                },
                    Constraint = new AndConstraint()
                {
                    new EqualsConstraint()
                    {
                        Property = "sys.type",
                        ValueToEqual = "Entry"
                    }
                }
                }
            };

            //Act
            var res = await _client.CreateRole(role);

            //Assert
            Assert.Equal("Developer", res.Name);
            Assert.Equal("sys.type", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).Property);
            Assert.Equal("Asset", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).ValueToEqual);
        }

        [Fact]
        public async Task RolesCollectionShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleRolesCollection.json");

            //Act
            var res = await _client.GetAllRoles();

            //Assert
            Assert.Equal(7, res.Total);
            Assert.Equal("Author", res.First().Name);
            Assert.Equal("create", res.First().Policies.First().Actions[0]);
            Assert.Equal("Translator 1", res.ElementAt(4).Name);
            Assert.Equal("Entry", ((res.ElementAt(4).Policies.First().Constraint as AndConstraint).First() as EqualsConstraint).ValueToEqual);
        }

        [Fact]
        public async Task CreateRoleShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleRole.json");

            var role = new Role()
            {
                Name = "Da role",
                Description = "Descriptive stuff",
                Permissions = new ContentfulPermissions()
                {
                    ContentDelivery = new List<string>()
                {
                    "all"
                },
                    ContentModel = new List<string>()
                {
                    "read"
                },
                    Settings = new List<string>()
                {
                    "read",
                    "manage"
                }
                }
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateRole(role);

            //Assert
            Assert.Equal(HttpMethod.Post, method);
            Assert.Equal("https://api.contentful.com/spaces/666/roles", url);
            Assert.Contains(@"""name"":""Da role""", contentSet);
            Assert.Contains(@"""description"":""Descriptive stuff""", contentSet);
            Assert.Contains(@"""ContentDelivery"":""all""", contentSet);
            Assert.Contains(@"""ContentModel"":[""read""]", contentSet);
            Assert.Contains(@"""Settings"":[""read"",""manage""]", contentSet);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateRoleShouldThrowForIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleRole.json");
            var role = new Role()
            {
                SystemProperties = new SystemProperties()
            };
            role.SystemProperties.Id = id;

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.UpdateRole(role));

            //Assert
            Assert.Equal("The id of the role must be set.", ex.Message);
        }

        [Theory]
        [InlineData("agsdg455")]
        [InlineData("324")]
        [InlineData("bcvb")]
        public async Task UpdateRoleShouldCallCorrectUrlWithData(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleRole.json");

            var role = new Role()
            {
                SystemProperties = new SystemProperties()
                {
                    Id = id
                },
                Name = "Rolemodel",
                Description = "Merry christmas!",
                Permissions = new ContentfulPermissions()
                {
                    ContentDelivery = new List<string>()
                {
                    "read","delete"
                },
                    ContentModel = new List<string>()
                {
                    "all"
                },
                    Settings = new List<string>()
                {
                    "read",
                    "manage"
                }
                }
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateRole(role);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/spaces/666/roles/{id}", url);
            Assert.Contains(@"""name"":""Rolemodel""", contentSet);
            Assert.Contains(@"""description"":""Merry christmas!""", contentSet);
            Assert.Contains(@"""ContentModel"":""all""", contentSet);
            Assert.Contains(@"""ContentDelivery"":[""read"",""delete""]", contentSet);
            Assert.Contains(@"""Settings"":[""read"",""manage""]", contentSet);
        }

        [Theory]
        [InlineData("09hfdh4-34")]
        [InlineData("643")]
        [InlineData("fdgs34")]
        public async Task DeletingRoleShouldCallCorrectUrl(string id)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteRole(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/roles/{id}", requestUrl);
        }

        [Fact]
        public async Task SnapshotsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SnapshotsCollection.json");

            //Act
            var res = await _client.GetAllSnapshotsForEntry("123");

            //Assert
            Assert.Equal("Seven Tips From Ernest Hemingway on How to Write Fiction", res.First().Fields["title"]["en-US"]);
            Assert.Equal("6U1xjMul1TRT2e1CiRwtLG", res.First().SystemProperties.Id);
            Assert.Equal("A96usFSlY4G0W4kwAqswk", res.First().EntityProperties.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetSnapshotShouldThrowForSnapshotIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshot.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetSnapshotForEntry(id, "something"));

            //Assert
            Assert.Equal($"The id of the snapshot must be set.{Environment.NewLine}Parameter name: snapshotId", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetSnapshotShouldThrowForEntryIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshot.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetSnapshotForEntry("something", id));

            //Assert
            Assert.Equal($"The id of the entry must be set.{Environment.NewLine}Parameter name: entryId", ex.Message);
        }

        [Fact]
        public async Task GetSnapshotShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshot.json");

            //Act
            var res = await _client.GetSnapshotForEntry("123", "wed");

            //Assert
            Assert.Equal("Somethi", res.Fields["field1"]["en-US"]);
            Assert.Equal("2ReMHJhXoAcy4AyamgsgwQ", res.Fields["field4"]["en-US"].sys.id.ToString());
            Assert.Equal("7u6sWOXikwxwr3cMrWddcl", res.SystemProperties.Id);
            Assert.Equal("42BwcCt4TeeskG0eq8S2CQ", res.EntityProperties.Id);
        }

        [Fact]
        public async Task GetSnapshotsForContentTypeShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContentTypeSnapshotsCollection.json");

            //Act
            var res = await _client.GetAllSnapshotsForContentType("123");
            var list = res.ToList();

            //Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(12, list.First().Snapshot.Fields.Count);
            Assert.Equal(13, list[1].Snapshot.Fields.Count);
            Assert.False(list[0].Snapshot.Fields.Any(c => c.Name == "test"));
            Assert.True(list[1].Snapshot.Fields.Any(c => c.Name == "test" && c.Omitted == true));
            Assert.True(list[2].Snapshot.Fields.Any(c => c.Name == "test" && c.Omitted == false));
            Assert.Equal("2pQBFoHZk5rMfGK1lSADTT", list[1].SystemProperties.Id);
            Assert.Equal("7b9PdWRHmlYce1YvZDOZdx", list[2].SystemProperties.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetSnapshotsForContentTypeShouldThrowForIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContentTypeSnapshotsCollection.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetAllSnapshotsForContentType(id));

            //Assert
            Assert.Equal($"The id of the content type must be set.{Environment.NewLine}Parameter name: contentTypeId", ex.Message);
        }

        [Theory]
        [InlineData("", "snap")]
        [InlineData(null, "snap")]
        [InlineData("cat", "")]
        [InlineData("dog", null)]
        public async Task GetSnapshotForContentTypeShouldThrowForIdNotSet(string id, string snapId)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshotContentType.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetSnapshotForContentType(snapId, id));

            //Assert
            Assert.Contains($"The id of the ", ex.Message);
        }

        [Fact]
        public async Task GetSnapshotForContentTypeShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshotContentType.json");

            //Act
            var res = await _client.GetSnapshotForContentType("123", "wed");

            //Assert
            Assert.Equal("Product", res.Snapshot.Name);
            Assert.Equal("productName", res.Snapshot.DisplayField);
            Assert.Equal(13, res.Snapshot.Fields.Count);
        }

        [Fact]
        public async Task SpaceMembershipsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpaceMembershipsCollection.json");

            //Act
            var res = await _client.GetSpaceMemberships();

            //Assert
            Assert.Equal(1, res.Total);
            Assert.True(res.First().Admin);
            Assert.Equal("123", res.First().Roles[1]);
        }

        [Fact]
        public async Task SpaceMembershipShouldSerializeCorrectly()
        {
            //Arrange
            var membership = new SpaceMembership()
            {
                Admin = true,
                Roles = new List<string>()
            {
                "123",
                "231",
                "12344"
            }
            };
            var serializedMembership = JsonConvert.SerializeObject(membership);
            _handler.Response = new HttpResponseMessage() { Content = new StringContent(serializedMembership) };

            //Act
            var res = await _client.CreateSpaceMembership(membership);

            //Assert
            Assert.True(res.Admin);
            Assert.Equal(3, res.Roles.Count);
            Assert.Equal("231", res.Roles[1]);
        }

        [Fact]
        public async Task CreateSpaceMembershipShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpaceMembershipsCollection.json");

            var spaceMembership = new SpaceMembership()
            {
                Admin = true,
                Roles = new List<string>()
            {
                "123",
                "342"
            }
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateSpaceMembership(spaceMembership);

            //Assert
            Assert.Equal(HttpMethod.Post, method);
            Assert.Equal("https://api.contentful.com/spaces/666/space_memberships", url);
            Assert.Equal(@"{""admin"":true,""roles"":[{""type"":""Link"",""linkType"":""Role"",""id"":""123""},{""type"":""Link"",""linkType"":""Role"",""id"":""342""}]}", contentSet);

        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateSpaceMembershipShouldThrowForEntryIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpaceMembershipsCollection.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetSnapshotForEntry("something", id));

            //Assert
            Assert.Equal($"The id of the entry must be set.{Environment.NewLine}Parameter name: entryId", ex.Message);
        }

        [Theory]
        [InlineData("afas23")]
        [InlineData("234")]
        [InlineData("bbs")]
        public async Task UpdateSpaceMembershipShouldCallCorrectUrlWithData(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSpaceMembershipsCollection.json");

            var spaceMembership = new SpaceMembership()
            {
                SystemProperties = new SystemProperties()
            };
            spaceMembership.SystemProperties.Id = id;
            spaceMembership.Admin = false;
            spaceMembership.Roles = new List<string>()
            {
                "43",
                "765"
            };
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateSpaceMembership(spaceMembership);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/spaces/666/space_memberships/{id}", url);
            Assert.Equal(@"{""admin"":false,""roles"":[{""type"":""Link"",""linkType"":""Role"",""id"":""43""},{""type"":""Link"",""linkType"":""Role"",""id"":""765""}]}", contentSet);
        }

        [Theory]
        [InlineData("adf-2345")]
        [InlineData("453")]
        [InlineData("agf")]
        public async Task DeletingSpaceMembershipShouldCallCorrectUrl(string id)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteSpaceMembership(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/space_memberships/{id}", requestUrl);
        }

        [Fact]
        public async Task GetApiKeysShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKeysCollection.json");

            //Act
            var res = await _client.GetAllApiKeys();

            //Assert
            Assert.Equal(2, res.Total);
            Assert.Equal(2, res.Count());
            Assert.Equal("123", res.First().AccessToken);
        }

        [Fact]
        public async Task GetApiKeyShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKey.json");

            //Act
            var res = await _client.GetApiKey("12");

            //Assert
            Assert.Equal("23423423456433", res.AccessToken);
        }

        [Fact]
        public async Task GetPreviewApiKeysShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKeysCollection.json");

            //Act
            var res = await _client.GetAllPreviewApiKeys();

            //Assert
            Assert.Equal(2, res.Total);
            Assert.Equal(2, res.Count());
            Assert.Equal("123", res.First().AccessToken);
        }

        [Fact]
        public async Task GetPreviewApiKeyShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKey.json");

            //Act
            var res = await _client.GetPreviewApiKey("12");

            //Assert
            Assert.Equal("23423423456433", res.AccessToken);
        }

        [Fact]
        public async Task CreateApiKeyShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKeysCollection.json");
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateApiKey("Key sharp!", "This is the desc");

            //Assert
            Assert.Equal(HttpMethod.Post, method);
            Assert.Equal("https://api.contentful.com/spaces/666/api_keys", url);
            Assert.Equal(@"{""name"":""Key sharp!"",""description"":""This is the desc""}", contentSet);
        }

        [Fact]
        public async Task UpdateApiKeyShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiKeysCollection.json");
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            var headerSet = false;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                headerSet = request.Headers.GetValues("X-Contentful-Version").First() == "5";
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateApiKey("some-id", "Key sharp2", "This is the desc again", 5);

            //Assert
            Assert.True(headerSet);
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal("https://api.contentful.com/spaces/666/api_keys/some-id", url);
            Assert.Equal(@"{""name"":""Key sharp2"",""description"":""This is the desc again""}", contentSet);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task UpdateApiKeyShouldThrowIfNoIdSet(string id)
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.UpdateApiKey(id, "Key sharp2", "This is the desc again", 5));

            //Assert
            Assert.Equal($"The id of the api key must be set.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Fact]
        public async Task UploadingFileShouldYieldCorrectResult()
        {
            //Arrange
            var fileBytes = new byte[] { 12, 43, 43, 54 };
            _handler.Response = GetResponseFromFile(@"UploadResult.json");

            //Act
            var res = await _client.UploadFile(fileBytes);

            //Assert
            Assert.IsType<UploadReference>(res);
            Assert.NotNull(res.SystemProperties.Id);
            Assert.Equal("666", res.SystemProperties.Id);
        }

        [Fact]
        public async Task GettingUploadedFileShouldYieldCorrectResult()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"UploadResult.json");

            //Act
            var res = await _client.GetUpload("5wAqke81S2C1o32RvTODCl");

            Assert.IsType<UploadReference>(res);
            Assert.NotNull(res.SystemProperties.Id);
            Assert.Equal("666", res.SystemProperties.Id);
        }

        [Fact]
        public async Task UploadedFileAndCreateAssetShouldYieldCorrectResult()
        {
            //Arrange
            var fileBytes = new byte[] { 12, 43, 43, 54 };
            _handler.Responses.Enqueue(GetResponseFromFile(@"UploadResult.json"));
            _handler.Responses.Enqueue(GetResponseFromFile(@"SampleAssetManagement.json"));
            _handler.Responses.Enqueue(new HttpResponseMessage());
            var asset = new ManagementAsset()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "356"
                },
                Files = new Dictionary<string, Core.Models.File>
                {
                    {  "en-US", new Core.Models.File() },
                    {  "sv-SE", new Core.Models.File() }
                }
            };
            //Act
            var res = await _client.UploadFileAndCreateAsset(asset, fileBytes);

            Assert.IsType<ManagementAsset>(res);
            Assert.NotNull(res.SystemProperties.Id);
            Assert.Equal("3S1ngcWajSia6I4sssQwyK", res.SystemProperties.Id);
        }

        [Fact]
        public async Task GetExtensionsShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtensionsCollection.json");

            //Act
            var res = await _client.GetAllExtensions();

            //Assert
            Assert.Equal(1, res.Total);
            Assert.Equal(1, res.Count());
            Assert.Equal("trul", res.First().Name);
        }

        [Fact]
        public async Task GetExtensionShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");

            //Act
            var res = await _client.GetExtension("B");

            //Assert
            Assert.Equal("trul", res.Name);
            Assert.Equal(2, res.FieldTypes.Count);
            Assert.Collection(res.FieldTypes,
               (t) => Assert.Equal("Symbol", t),
               (t) => Assert.Equal("Text", t));
            Assert.Equal("https://robertlinde.se", res.Src);
        }

        [Fact]
        public async Task CreateExtensionShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var ext = new UiExtension()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "1234"
                },
                Name = "trul"
            };
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateExtension(ext);



            //Assert
            Assert.DoesNotContain("sys", contentSet);
            Assert.Equal("trul", res.Name);
            Assert.Equal(2, res.FieldTypes.Count);
            Assert.Collection(res.FieldTypes,
               (t) => Assert.Equal("Symbol", t),
               (t) => Assert.Equal("Text", t));
            Assert.Equal("https://robertlinde.se", res.Src);
        }

        [Fact]
        public async Task CreateExtensionWithParametersShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var ext = new UiExtension()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "1234"
                },
                Name = "trul",
                FieldTypes = new List<string> { "Symbol", "Text" }
            };

            ext.Parameters = new UiExtensionParametersLists
            {
                InstanceParameters = new List<UiExtensionParameters>
                {
                    new UiExtensionParameters
                    {
                        Id = "test",
                        Name = "Test",
                        Type = UiExtensionParameterTypes.Symbol,
                        Description = "This is the description",
                        Default = "nutty"
                    }
                }
            };

            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateExtension(ext);

            //Assert
            Assert.DoesNotContain("sys", contentSet);
            Assert.Equal("trul", res.Name);
            Assert.Equal(2, res.FieldTypes.Count);
            Assert.Collection(res.FieldTypes,
               (t) => Assert.Equal("Symbol", t),
               (t) => Assert.Equal("Text", t));
            Assert.Equal("https://robertlinde.se", res.Src);
        }

        [Fact]
        public async Task CreateOrUpdateExtensionShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var ext = new UiExtension()
            {
                SystemProperties = new SystemProperties
                {
                    Id = "bob"
                },
                Name = "trul"
            };
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrUpdateExtension(ext);

            //Assert
            Assert.DoesNotContain("sys", contentSet);
            Assert.Equal("trul", res.Name);
            Assert.Equal(2, res.FieldTypes.Count);
            Assert.Collection(res.FieldTypes,
               (t) => Assert.Equal("Symbol", t),
               (t) => Assert.Equal("Text", t));
            Assert.Equal("https://robertlinde.se", res.Src);
        }

        [Fact]
        public async Task CreateOrUpdateExtensionShouldThrowIfNoIdSet()
        {
            //Arrange
            var ext = new UiExtension()
            {
                Name = "trul"
            };
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateExtension(ext));

            //Assert
            Assert.Equal($"The id of the extension must be set.{Environment.NewLine}Parameter name: extension", ex.Message);
        }

        [Theory]
        [InlineData("552")]
        [InlineData("wer324")]
        [InlineData("xn3315af")]
        public async Task DeletingExtensionShouldCallCorrectUrl(string id)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteExtension(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/extensions/{id}", requestUrl);
        }

        [Fact]
        public async Task CreatingATokenShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"CreatedToken.json");
            var token = new ManagementToken()
            {
                Name = "Brandao",
                Scopes = new List<string>
                {
                    "content_management_manage"
                }
            };

            //Act
            var res = await _client.CreateManagementToken(token);

            //Assert
            Assert.Equal("My Token", res.Name);
            Assert.Collection(res.Scopes, (c) => { Assert.Equal(SystemManagementScopes.Manage, c); });
            Assert.Equal("46d42a80f96db00393ffa867a753de126e658484dd8a20d209bcb7efcf3761b9", res.Token);
        }

        [Fact]
        public async Task GettingATokenShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"GetToken.json");

            //Act
            var res = await _client.GetManagementToken("pop");

            //Assert
            Assert.Equal("My Token", res.Name);
            Assert.Collection(res.Scopes, (c) => { Assert.Equal(SystemManagementScopes.Read, c); });
            Assert.Null(res.Token);
            Assert.Null(res.RevokedAt);
        }

        [Fact]
        public async Task GetTokensShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"TokensCollection.json");

            //Act
            var res = await _client.GetAllManagementTokens();

            //Assert

            Assert.Collection(res, (c) =>
            {
                Assert.Equal(SystemManagementScopes.Manage, c.Scopes.First());
                Assert.Equal(1, c.Scopes.Count);
                Assert.Null(c.Token);
                Assert.Equal("My Token", c.Name);
            });
        }

        [Fact]
        public async Task RevokingATokenShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"RevokedToken.json");

            //Act
            var res = await _client.RevokeManagementToken("pop");

            //Assert
            Assert.Equal("My Token", res.Name);
            Assert.Collection(res.Scopes, (c) => { Assert.Equal(SystemManagementScopes.Manage, c); });
            Assert.Null(res.Token);
            Assert.NotNull(res.RevokedAt);
        }

        [Fact]
        public async Task GettingTheCurrentUserShouldReturnCorrectResult()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleUser.json");
            //Act
            var res = await _client.GetCurrentUser();

            //Assert
            Assert.Equal("https://images.contentful.com/abcd1234", res.AvatarUrl);
        }

        [Fact]
        public async Task GettingOrganizationShouldReturnCorrectResult()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"OrganizationsCollection.json");
            //Act
            var res = await _client.GetOrganizations();

            //Assert
            Assert.Collection(res,
                (s) => { Assert.Equal("Silvertip", s.Name); },
                (s) => { Assert.Equal("CompanyBob", s.Name); },
                (s) => { Assert.Equal("CompanyAlice", s.Name); }
                );
        }

        [Fact]
        public async Task SerializedObjectShouldBeProperlyCapitalized()
        {
            //Arrange
            var fields = new CamelTest
            {
                NotCamel = "Not a camel",
                NotACamelEither = "Neither is this!",
                LongThing = "This is though, a pure camel!"
            };

            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var entry = new Entry<dynamic>
            {
                Fields = fields
            };
            var contentSet = "";
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateEntry(entry, "bluemoon");

            //Assert
            Assert.Contains("NotCamelISay", contentSet);
            Assert.Contains("NoCamelHere", contentSet);
            Assert.Contains("long", contentSet);
        }

        [Fact]
        public async Task SettingEnvironmentForContentTypesShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetContentTypes();

            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/", path);
        }

        [Fact]
        public async Task SettingEnvironmentAndQueryContentTypesShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetContentTypes("?limit=1000", "564");

            //Assert
            Assert.Equal(5, res.Count());
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/?limit=1000", path);
        }

        [Fact]
        public async Task SettingEnvironmentForCreatingContentTypesShouldYieldCorrectUrl()
        {
            //Arrange
            var contentType = new ContentType()
            {
                Name = "Barbossa",
                SystemProperties = new SystemProperties()
            };
            contentType.SystemProperties.Id = "323";
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateOrUpdateContentType(contentType);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/323", path);
        }

        [Fact]
        public async Task SettingEnvironmentForSingleContentTypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentType.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetContentType("123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123", path);
        }

        [Fact]
        public async Task SettingEnvironmentForDeleteContentTypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeleteContentType("123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123", path);
        }

        [Fact]
        public async Task SettingEnvironmentForActivateContentTypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.ActivateContentType("123", 2);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentForDeactivateContentTypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeactivateContentType("123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentForGetActivatedContentTypesShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContenttypesCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.GetActivatedContentTypes();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/public/content_types/", path);
        }

        [Fact]
        public async Task SettingEnvironmentForGettingEditorInterfaceShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetEditorInterface("123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123/editor_interface", path);
        }

        [Fact]
        public async Task SettingEnvironmentForUpdatingEditorInterfaceShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EditorInterface.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.UpdateEditorInterface(new EditorInterface(), "123", 2);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/123/editor_interface", path);
        }

        [Fact]
        public async Task SettingEnvironmentGettingEntriesShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EntriesCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetEntriesCollection<dynamic>();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries", path);
        }

        [Fact]
        public async Task SettingEnvironmentForCreatingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateEntry(new Entry<dynamic>(), "123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries", path);
        }

        [Fact]
        public async Task SettingEnvironmentForCreatingOrUpdatingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateOrUpdateEntry(new Entry<dynamic> { SystemProperties = new SystemProperties { Id = "323" } }, version: 123);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/323", path);
        }

        [Fact]
        public async Task SettingEnvironmentForGettingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetEntry("123");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123", path);
        }

        [Fact]
        public async Task SettingEnvironmentForDeletingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeleteEntry("123", 32);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123", path);
        }

        [Fact]
        public async Task SettingEnvironmentForPublishingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.PublishEntry("123", 43);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentForUnpublishingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.UnpublishEntry("123", 32);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentForArchivingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.ArchiveEntry("123", 32);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123/archived", path);
        }

        [Fact]
        public async Task SettingEnvironmentForUnarchivingEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEntryManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.UnarchiveEntry("123", 32);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/123/archived", path);
        }

        [Fact]
        public async Task SettingEnvironmentGettingAssetsShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetAssetsCollection();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/", path);
        }

        [Fact]
        public async Task SettingEnvironmentGettingPublishedAssetsShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"AssetsCollectionManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetPublishedAssetsCollection();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/public/assets", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetAsset("343");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343", path);
        }

        [Fact]
        public async Task SettingEnvironmentDeleteAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeleteAsset("343", 4);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343", path);
        }

        [Fact]
        public async Task SettingEnvironmentPublishAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.PublishAsset("343", 45);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentUnpublishAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.UnpublishAsset("343", 45);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343/published", path);
        }

        [Fact]
        public async Task SettingEnvironmentArchiveAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.ArchiveAsset("343", 45);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343/archived", path);
        }

        [Fact]
        public async Task SettingEnvironmentUnarchiveAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.UnarchiveAsset("343", 45);

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343/archived", path);
        }

        [Fact]
        public async Task SettingEnvironmentProcessAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.ProcessAsset("343", 47, "en-US");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/343/files/en-US/process", path);
        }

        [Fact]
        public async Task SettingEnvironmentCreateOrUpdateAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateOrUpdateAsset(new ManagementAsset { SystemProperties = new SystemProperties { Id = "325" } });

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets/325", path);
        }

        [Fact]
        public async Task SettingEnvironmentCreateAssetShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleAssetManagement.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateAsset(new ManagementAsset());

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/assets", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetLocalesShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"LocalesCollection.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetLocalesCollection();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/locales", path);
        }

        [Fact]
        public async Task SettingEnvironmentCreateLocaleShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateLocale(new Locale());

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/locales", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetLocaleShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetLocale("en-US");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/locales/en-US", path);
        }

        [Fact]
        public async Task SettingEnvironmentUpdateLocaleShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleLocale.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.UpdateLocale(new Locale() { SystemProperties = new SystemProperties { Id = "en-US" } });

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/locales/en-US", path);
        }

        [Fact]
        public async Task SettingEnvironmentDeleteLocaleShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeleteLocale("en-US");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/locales/en-US", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetAllSnapshotsForEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SnapshotsCollection.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetAllSnapshotsForEntry("4343");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/4343/snapshots", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetSnapshotForEntryShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshot.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetSnapshotForEntry("666", "4343");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/entries/4343/snapshots/666", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetAllSnapshotsForContentTypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ContentTypeSnapshotsCollection.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetAllSnapshotsForContentType("abc");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/abc/snapshots", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetSnapshotForContenttypeShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleSnapshotContentType.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetSnapshotForContentType("432", "bdb");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/content_types/bdb/snapshots/432", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetAllExtensionsShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtensionsCollection.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetAllExtensions();

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/extensions", path);
        }

        [Fact]
        public async Task SettingEnvironmentCreateExtensionsShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateExtension(new UiExtension());

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/extensions", path);
        }

        [Fact]
        public async Task SettingEnvironmentCreateOrUpdateExtensionsShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.CreateOrUpdateExtension(new UiExtension() { SystemProperties = new SystemProperties { Id = "t4t" } });

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/extensions/t4t", path);
        }

        [Fact]
        public async Task SettingEnvironmentGetExtensionShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleExtension.json");
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            var res = await client.GetExtension("gegjy");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/extensions/gegjy", path);
        }

        [Fact]
        public async Task SettingEnvironmentDeleteExtensionShouldYieldCorrectUrl()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();
            var client = GetClientWithEnvironment();
            var path = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                path = request.RequestUri.ToString();
            };

            //Act
            await client.DeleteExtension("sbth");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/564/environments/special/extensions/sbth", path);
        }

        [Fact]
        public async Task GettingEnvironmentsShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"EnvironmentsCollection.json");

            //Act
            var res = await _client.GetEnvironments();

            //Assert
            Assert.Equal(2, res.Total);
            Assert.Collection(res,
                (e) =>
                {
                    Assert.Equal("queued", e.SystemProperties.Status.SystemProperties.Id);
                    Assert.Equal("Link", e.SystemProperties.Status.SystemProperties.Type);
                    Assert.Equal("Status", e.SystemProperties.Status.SystemProperties.LinkType);
                },
                (e) =>
                {
                    Assert.Equal("queued", e.SystemProperties.Status.SystemProperties.Id);
                    Assert.Equal("Link", e.SystemProperties.Status.SystemProperties.Type);
                    Assert.Equal("Status", e.SystemProperties.Status.SystemProperties.LinkType);
                }
                );
        }

        [Fact]
        public async Task CreateEnvironmentShouldCallCorrectUrlWithCorrectData()
        {
            //Arrange

            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.CreateEnvironment("theenv");
            //Assert
            Assert.Equal(HttpMethod.Post, requestMethod);
            Assert.Equal("https://api.contentful.com/spaces/666/environments", requestUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreateEnvironmentByIdShouldThrowIfIdNotSet(string s)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateEnvironment(s, ""));
            //Assert
            Assert.Equal($"You must provide an id for the environment.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CloneEnvironmentShouldThrowIfIdNotSet(string s)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CloneEnvironment(s, "", ""));
            //Assert
            Assert.Equal($"You must provide an id for the environment.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CloneEnvironmenShouldThrowIfNameNotSet(string s)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CloneEnvironment("pop", s, ""));
            //Assert
            Assert.Equal($"You must provide a name for the environment.{Environment.NewLine}Parameter name: name", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CloneEnvironmenShouldThrowISourceNotSet(string s)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CloneEnvironment("pop", "bop", s));
            //Assert
            Assert.Equal($"You must provide an id for the source environment.{Environment.NewLine}Parameter name: sourceEnvironmentId", ex.Message);
        }

        [Fact]
        public async Task CloneEnvironmentShouldCallCorrectUrlWithCorrectData()
        {
            //Arrange

            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            var header = "";
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                header = request.Headers.GetValues("X-Contentful-Source-Environment").First();
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.CloneEnvironment("bob", "pop", "canasta");
            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("canasta", header);
            Assert.Equal("https://api.contentful.com/spaces/666/environments/bob", requestUrl);
        }

        [Fact]
        public async Task CreateEnvironmentByIdShouldCallCorrectUrlWithCorrectData()
        {
            //Arrange

            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.CreateOrUpdateEnvironment("bob", "pop");
            //Assert
            Assert.Equal(HttpMethod.Put, requestMethod);
            Assert.Equal("https://api.contentful.com/spaces/666/environments/bob", requestUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetEnvironmentByIdShouldThrowIfIdNotSet(string s)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEnvironment(s));
            //Assert
            Assert.Equal($"You must provide an id for the environment.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Fact]
        public async Task GetEnvironmentShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");

            //Act
            var res = await _client.GetEnvironment("hehe");

            //Assert
            Assert.Equal("queued", res.SystemProperties.Status.SystemProperties.Id);
            Assert.Equal("Link", res.SystemProperties.Status.SystemProperties.Type);
            Assert.Equal("Status", res.SystemProperties.Status.SystemProperties.LinkType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task DeleteEnvironmentByIdShouldThrowIfIdNotSet(string s)
        {
            //Arrange
            _handler.Response = new HttpResponseMessage();

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.DeleteEnvironment(s));
            //Assert
            Assert.Equal($"You must provide an id for the environment.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Fact]
        public async Task DeleteEnvironmentShouldCallCorrectURl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleEnvironment.json");
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteEnvironment("hehe");

            //Assert
            Assert.Equal("https://api.contentful.com/spaces/666/environments/hehe", requestUrl);
            Assert.Equal(HttpMethod.Delete, requestMethod);
        }

        [Fact]
        public async Task GetUsagePeriodsShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"UsagePeriods.json");

            //Act
            var res = await _client.GetUsagePeriods("some-id");

            //Assert
            Assert.Collection(res,
                    (t) =>
                    {
                        Assert.Equal("07/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Null(t.EndDate);
                        Assert.Equal("4", t.SystemProperties.Id);
                    },
                    (t) =>
                    {
                        Assert.Equal("06/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("07/14/2018 00:00:00", t.EndDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("3", t.SystemProperties.Id);
                    },
                    (t) =>
                    {
                        Assert.Equal("05/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("06/14/2018 00:00:00", t.EndDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("2", t.SystemProperties.Id);
                    },
                    (t) =>
                    {
                        Assert.Equal("04/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("05/14/2018 00:00:00", t.EndDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("1", t.SystemProperties.Id);
                    }
                );
        }

        [Fact]
        public async Task GetResourceUsageShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"ApiUsage.json");

            //Act
            var res = await _client.GetResourceUsage("some-id", "organization");

            //Assert
            Assert.Collection(res,
                    (t) =>
                    {
                        Assert.Equal("06/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("07/14/2018 00:00:00", t.EndDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("20_1ElgCn1mi1UHSBLTP2v4TD", t.SystemProperties.Id);
                        Assert.Equal("daily", t.Interval);
                        Assert.Equal("apiRequests", t.UnitOfMeasure);
                        Assert.Collection(t.Usage,
                                (x) => Assert.Equal(6442, x),
                                (x) => Assert.Equal(5952, x),
                                (x) => Assert.Equal(8713, x),
                                (x) => Assert.Equal(1998, x),
                                (x) => Assert.Equal(5209, x),
                                (x) => Assert.Equal(2894, x),
                                (x) => Assert.Equal(1144, x),
                                (x) => Assert.Equal(2152, x),
                                (x) => Assert.Equal(4305, x),
                                (x) => Assert.Equal(4534, x),
                                (x) => Assert.Equal(1832, x),
                                (x) => Assert.Equal(5979, x),
                                (x) => Assert.Equal(6189, x),
                                (x) => Assert.Equal(954, x),
                                (x) => Assert.Equal(4847, x),
                                (x) => Assert.Equal(3953, x),
                                (x) => Assert.Equal(9094, x),
                                (x) => Assert.Equal(1800, x),
                                (x) => Assert.Equal(5044, x),
                                (x) => Assert.Equal(1232, x),
                                (x) => Assert.Equal(920, x),
                                (x) => Assert.Equal(2396, x),
                                (x) => Assert.Equal(9551, x),
                                (x) => Assert.Equal(4350, x),
                                (x) => Assert.Equal(2809, x),
                                (x) => Assert.Equal(5496, x),
                                (x) => Assert.Equal(9442, x),
                                (x) => Assert.Equal(7301, x),
                                (x) => Assert.Equal(4349, x),
                                (x) => Assert.Equal(1778, x)
                            );
                    },
                    (t) =>
                    {
                        Assert.Equal("06/15/2018 00:00:00", t.StartDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("07/14/2018 00:00:00", t.EndDate.Value.ToString(CultureInfo.InvariantCulture));
                        Assert.Equal("20", t.SystemProperties.UsagePeriod.SystemProperties.Id);
                        Assert.Equal("20_1ElgCn1mi1UHSBLTP2v5TD", t.SystemProperties.Id);
                        Assert.Equal("daily", t.Interval);
                        Assert.Equal("apiRequests", t.UnitOfMeasure);
                        Assert.Collection(t.Usage,
                                (x) => Assert.Equal(6444, x),
                                (x) => Assert.Equal(5955, x),
                                (x) => Assert.Equal(8716, x),
                                (x) => Assert.Equal(1998, x),
                                (x) => Assert.Equal(5209, x),
                                (x) => Assert.Equal(2894, x),
                                (x) => Assert.Equal(1144, x),
                                (x) => Assert.Equal(2152, x),
                                (x) => Assert.Equal(4305, x),
                                (x) => Assert.Equal(4534, x),
                                (x) => Assert.Equal(1832, x),
                                (x) => Assert.Equal(5979, x),
                                (x) => Assert.Equal(6189, x),
                                (x) => Assert.Equal(954, x),
                                (x) => Assert.Equal(4847, x),
                                (x) => Assert.Equal(3953, x),
                                (x) => Assert.Equal(9094, x),
                                (x) => Assert.Equal(1800, x),
                                (x) => Assert.Equal(5044, x),
                                (x) => Assert.Equal(1232, x),
                                (x) => Assert.Equal(920, x),
                                (x) => Assert.Equal(2396, x),
                                (x) => Assert.Equal(9551, x),
                                (x) => Assert.Equal(4350, x),
                                (x) => Assert.Equal(2809, x),
                                (x) => Assert.Equal(5496, x),
                                (x) => Assert.Equal(9442, x),
                                (x) => Assert.Equal(7301, x),
                                (x) => Assert.Equal(4349, x),
                                (x) => Assert.Equal(1778, x)
                            );
                    }
                );
        }

        public async Task OrganizationMembershipsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleOrgMembershipsCollection.json");

            //Act
            var res = await _client.GetOrganizationMemberships("123");

            //Assert
            Assert.Equal(2, res.Total);
            Assert.Equal("admin", res.First().Role);
        }

        [Fact]
        public async Task CreateOrganizationMembershipShouldCallCorrectUrlWithData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleOrgMembershipsCollection.json");

            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrganizationMembership("666", "member", "bob@sob.com", false);

            //Assert
            Assert.Equal(HttpMethod.Post, method);
            Assert.Equal("https://api.contentful.com/organizations/666/organization_memberships", url);
            Assert.Equal(@"{""role"":""member"",""email"":""bob@sob.com"",""suppressInvitation"":false}", contentSet);

        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateOrganizationMembershipShouldThrowForEntryIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleOrgMembershipsCollection.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.UpdateOrganizationMembership("something", id, "org"));

            //Assert
            Assert.Equal($"The id of the organization membership must be set.{Environment.NewLine}Parameter name: membershipId", ex.Message);
        }

        [Theory]
        [InlineData("afas23")]
        [InlineData("234")]
        [InlineData("bbs")]
        public async Task UpdateOrganizationMembershipShouldCallCorrectUrlWithData(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleOrgMembershipsCollection.json");

            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateOrganizationMembership("bob", id, "orgolonio");

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal($"https://api.contentful.com/organizations/orgolonio/organization_memberships/{id}", url);
            Assert.Equal(@"{""role"":""bob""}", contentSet);
        }

        [Theory]
        [InlineData("adf-2345", "F2f")]
        [InlineData("453", "fdadf")]
        [InlineData("agf", "canberra")]
        public async Task DeletingOrganizationMembershipShouldCallCorrectUrl(string id, string orgId)
        {
            //Arrange
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.Response = new HttpResponseMessage();
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeleteOrganizationMembership(id, orgId);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/organizations/{orgId}/organization_memberships/{id}", requestUrl);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateContentTagShouldThrowForIdNotSet(string id)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateContentTag("something", id, false));

            //Assert
            Assert.Equal($"The id of the content tag must be set.{Environment.NewLine}Parameter name: id", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateContentTagShouldThrowForNameNotSet(string name)
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateContentTag(name, "something", false));

            //Assert
            Assert.Equal($"The name of the content tag must be set.{Environment.NewLine}Parameter name: name", ex.Message);
        }

        [Fact]
        public async Task UpdateContentTagShouldCallCorrectUrlAndReturnCorrectData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            var headerSet = false;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                headerSet = request.Headers.GetValues("X-Contentful-Version").First() == "5";
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateContentTag("badabing", "bob", version: 5);

            //Assert
            Assert.True(headerSet);
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal("https://api.contentful.com/spaces/666/tags/bob", url);
            Assert.Contains(@"""name"":""badabing""", contentSet);
            Assert.Contains(@"""id"":""bob""", contentSet);
            Assert.Equal("NY Campaign", res.Name);
            Assert.Equal("nyCampaign", res.SystemProperties.Id);
        }

        [Fact]
        public async Task CreateContentTagShouldCallCorrectUrlAndReturnCorrectData()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateContentTag("badabing", "bob", true);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal("https://api.contentful.com/spaces/666/tags/bob", url);
            Assert.Contains(@"""name"":""badabing""", contentSet);
            Assert.Contains(@"""id"":""bob""", contentSet);
            Assert.Contains(@"""visibility"":""public""", contentSet);
            Assert.Equal("NY Campaign", res.Name);
            Assert.Equal("nyCampaign", res.SystemProperties.Id);
        }

        [Fact]
        public async Task CreateContentTagShouldCallCorrectUrlAndReturnCorrectDataForPrivateTag()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");
            var contentSet = "";
            var url = "";
            var method = HttpMethod.Trace;
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                method = request.Method;
                url = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateContentTag("badabing", "bob", false);

            //Assert
            Assert.Equal(HttpMethod.Put, method);
            Assert.Equal("https://api.contentful.com/spaces/666/tags/bob", url);
            Assert.Contains(@"""name"":""badabing""", contentSet);
            Assert.Contains(@"""id"":""bob""", contentSet);
            Assert.Contains(@"""visibility"":""private""", contentSet);
            Assert.Equal("NY Campaign", res.Name);
            Assert.Equal("nyCampaign", res.SystemProperties.Id);
        }

        [Fact]
        public async Task GetContentTagCollectionShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTagCollection.json");

            //Act
            var res = await _client.GetContentTagsCollection();

            //Assert
            Assert.Equal(1, res.Total);
            Assert.Equal(1, res.Count());
            Assert.Equal("NY Campaign", res.First().Name);
            Assert.Equal("nyCampaign", res.First().SystemProperties.Id);
        }

        [Fact]
        public async Task GetContentTagShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"SampleContentTag.json");

            //Act
            var res = await _client.GetContentTag("id");

            //Assert
            Assert.Equal("NY Campaign", res.Name);
            Assert.Equal("nyCampaign", res.SystemProperties.Id);
        }

        private ContentfulManagementClient GetClientWithEnvironment(string env = "special")
        {
            var httpClient = new HttpClient(_handler);
            var options = new ContentfulOptions
            {
                ManagementApiKey = "123",
                Environment = env,
                SpaceId = "564"
            };
            var client = new ContentfulManagementClient(httpClient, options);
            return client;
        }
    }
}
