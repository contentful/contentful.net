using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Contentful.Core.Tests
{
    public class ContentfulManagementClientTests
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
        public void CreatingManagementClientShouldSetHeadersCorrectly()
        {
            //Arrange
            
            //Act
            //Assert
            Assert.Equal("564", _httpClient.DefaultRequestHeaders.Authorization.Parameter);
        }
        
        [Fact]
        public async Task CreateContentTypeShouldSerializeRequestCorrectly()
        {
            //Arrange
            _handler.Response = new HttpResponseMessage() {
                Content = new StringContent("{}")
            };
            var contentType = new ContentType();
            contentType.SystemProperties = new SystemProperties();
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
            var res = await _client.CreateOrUpdateContentTypeAsync(contentType);

            //Assert
            Assert.Null(res.Name);
        }

        [Fact]
        public async Task EditorInterfaceShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EditorInterface.json");

            //Act
            var res = await _client.GetEditorInterfaceAsync("someid");

            //Assert
            Assert.Equal(7, res.Controls.Count);
            Assert.IsType<BooleanEditorInterfaceControlSettings>(res.Controls[4].Settings);
        }

        [Fact]
        public async Task CreateSpaceShouldCreateCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleSpace.json");
            var headerSet = false;
            var contentSet = "";
            _handler.VerificationBeforeSend = () =>
            {
                 headerSet = _httpClient.DefaultRequestHeaders.GetValues("X-Contentful-Organization").First() == "112";
            };
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateSpaceAsync("Spaceman", "en-US", "112");

            //Assert
            Assert.True(headerSet);
            Assert.Equal(@"{""name"":""Spaceman"",""defaultLocale"":""en-US""}", contentSet);
            Assert.Equal("Products", res.Name);
        }

        [Fact]
        public async Task UpdateSpaceShouldCreateCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleSpace.json");
            var headerSet = false;
            var contentSet = "";
            _handler.VerificationBeforeSend = () =>
            {
                headerSet = _httpClient.DefaultRequestHeaders.GetValues("X-Contentful-Version").First() == "37";
            };
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.UpdateSpaceNameAsync("spaceId", "Spacemaster", 37, "333");

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
            await _client.DeleteSpaceAsync(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/{id}", requestUrl);
        }

        [Fact]
        public async Task GetContentTypesShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\ContenttypesCollectionManagement.json");
            //Act
            var res = await _client.GetContentTypesAsync();

            //Assert
            Assert.Equal(4, res.Count());
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);
        }

        [Fact]
        public async Task CreateOrUpdateContentTypeShouldThrowIfNoIdSet()
        {
            //Arrange
            var contentType = new ContentType();
            contentType.Name = "Barbossa";
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.CreateOrUpdateContentTypeAsync(contentType));

            //Assert
            Assert.Equal("The id of the content type must be set.\r\nParameter name: contentType", ex.Message);
        }

        [Fact]
        public async Task CreateOrUpdateContentTypeShouldCreateCorrectObject()
        {
            //Arrange
            var contentType = new ContentType();
            contentType.Name = "Barbossa";
            contentType.SystemProperties = new SystemProperties();
            contentType.SystemProperties.Id = "323";
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleContentType.json");

            var versionHeader = "";
            var contentSet = "";
            _handler.VerificationBeforeSend = () =>
            {
                versionHeader = _httpClient.DefaultRequestHeaders.GetValues("X-Contentful-Version").First();
            };
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };

            //Act
            var res = await _client.CreateOrUpdateContentTypeAsync(contentType, version: 22);

            //Assert
            Assert.Equal("22", versionHeader);
            Assert.Contains(@"""name"":""Barbossa""", contentSet);
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
        public async Task GetContentTypeShouldThrowForEmptyId()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleContentType.json");

            //Act
            var res = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetContentTypeAsync(""));

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
            await _client.DeleteContentTypeAsync(id);

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/content_types/{id}", requestUrl);
        }

        [Fact]
        public async Task ActivatingContentTypeShouldThrowForEmptyId()
        {
            //Arrange
            
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.ActivateContentTypeAsync("", 34));

            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task ActivatingContentTypeShouldCallCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleContentType.json");

            var versionHeader = "";
            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;
            _handler.VerificationBeforeSend = () =>
            {
                versionHeader = _httpClient.DefaultRequestHeaders.GetValues("X-Contentful-Version").First();
            };
            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.ActivateContentTypeAsync("758", 345);

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
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.DeactivateContentTypeAsync(""));

            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task DeactivatingContentTypeShouldCallCorrectUrl()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleContentType.json");

            var requestUrl = "";
            var requestMethod = HttpMethod.Trace;

            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestMethod = request.Method;
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            await _client.DeactivateContentTypeAsync("324");

            //Assert
            Assert.Equal(HttpMethod.Delete, requestMethod);
            Assert.Equal($"https://api.contentful.com/spaces/666/content_types/324/published", requestUrl);
        }

        [Fact]
        public async Task GetActivatedContentTypesShouldSerializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\ContenttypesCollectionManagement.json");
            var requestUrl = "";

            _handler.VerifyRequest = (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
            };

            //Act
            var res = await _client.GetActivatedContentTypesAsync();

            //Assert
            Assert.Equal(4, res.Count());
            Assert.Equal($"https://api.contentful.com/spaces/666/public/content_types", requestUrl);
            Assert.Equal("someName", res.First().Name);
            Assert.Equal(8, (res.First().Fields.First().Validations.First() as SizeValidator).Max);

        }

        [Fact]
        public async Task GetEditorInterfaceShouldThrowForEmptyId()
        {
            //Arrange

            //Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.GetEditorInterfaceAsync(""));
            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task GetEditorInterfaceShouldReturnCorrectObject()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\EditorInterface.json");

            //Act
            var res = await _client.GetEditorInterfaceAsync("23");
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
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _client.UpdateEditorInterfaceAsync(editorInterface, "", 6));
            //Assert
            Assert.Equal("contentTypeId", ex.Message);
        }

        [Fact]
        public async Task UpdateEditorInterfaceShouldCallCorrectUrl()
        {
            //Arrange
            var editorInterface = new EditorInterface();
            editorInterface.Controls = new List<EditorInterfaceControl>()
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
            };
            var versionHeader = "";
            var contentSet = "";
            var requestUrl = "";
            _handler.VerificationBeforeSend = () =>
            {
                versionHeader = _httpClient.DefaultRequestHeaders.GetValues("X-Contentful-Version").First();
            };
            _handler.VerifyRequest = async (HttpRequestMessage request) =>
            {
                requestUrl = request.RequestUri.ToString();
                contentSet = await (request.Content as StringContent).ReadAsStringAsync();
            };
            _handler.Response = GetResponseFromFile(@"JsonFiles\EditorInterface.json");

            //Act
            var res = await _client.UpdateEditorInterfaceAsync(editorInterface, "123", 16);

            //Assert
            Assert.Equal("16", versionHeader);
            Assert.Equal("https://api.contentful.com/spaces/666/content_types/123/editor_interface", requestUrl);
            Assert.Equal(@"{""controls"":[{""fieldId"":""field1"",""widgetId"":""singleLine""},{""fieldId"":""field2"",""widgetId"":""boolean"",""settings"":{""trueLabel"":""Truthy"",""falseLabel"":""Falsy"",""helpText"":""Help me here!""}}]}", contentSet);
        }

        [Fact]
        public async Task EditorInterfaceShouldSerializeCorrectly()
        {
            //Arrange
            var editorInterface = new EditorInterface();
            editorInterface.Controls = new List<EditorInterfaceControl>()
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
            };
            _handler.Response = GetResponseFromFile(@"JsonFiles\EditorInterface.json");

            //Act
            var res = await _client.UpdateEditorInterfaceAsync(editorInterface, "123", 1);

            //Assert
            Assert.Equal(7, res.Controls.Count);
            Assert.IsType<BooleanEditorInterfaceControlSettings>(res.Controls[4].Settings);
        }

        [Fact]
        public async Task WebHookCallDetailsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\WebhookCallDetails.json");
            //Act
            var res = await _client.GetWebHookCallDetailsAsync("b", "s");

            //Assert
            Assert.Equal("unarchive", res.EventType);
            Assert.Equal("close", res.Response.Headers["connection"]);

        }

        [Fact]
        public void ConstraintsShouldSerializeCorrectly()
        {
            //Arrange
            var policy = new Policy();
            policy.Effect = "allow";
            policy.Actions = new List<string>()
            {
                "create",
                "delete"
            };
            policy.Constraint =
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
            };

            //Act
            string output = JsonConvert.SerializeObject(policy, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            //Assert
            Assert.Equal(@"{""effect"":""allow"",""actions"":[""create"",""delete""],""constraint"":{""and"":[{""equals"":[{""doc"":""sys.type""},""Entry""]},{""equals"":[{""doc"":""sys.createdBy.sys.id""},""User.current()""]},{""not"":{""equals"":[{""doc"":""sys.contentType.sys.id""},""123""]}}]}}", output);
        }

        [Fact]
        public async Task RolesShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleRole.json");

            //Act
            var res = await _client.GetRoleAsync("123");

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
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleRole.json");


            role.Name = "test";
            role.Description = "desc";
            role.Permissions = new ContentfulPermissions();
            role.Permissions.ContentDelivery = new List<string>() { "all" };
            role.Permissions.ContentModel = new List<string>() { "read" };
            role.Permissions.Settings = new List<string>() { "read", "manage" };
            role.Policies = new List<Policy>();
            role.Policies.Add(new Policy()
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
            });

            //Act
            var res = await _client.CreateRoleAsync(role);

            //Assert
            Assert.Equal("Developer", res.Name);
            Assert.Equal("sys.type", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).Property);
            Assert.Equal("Asset", ((res.Policies[1].Constraint as AndConstraint)[0] as EqualsConstraint).ValueToEqual);
        }

        [Fact]
        public async Task RolesCollectionShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleRolesCollection.json");

            //Act
            var res = await _client.GetAllRolesAsync();

            //Assert
            Assert.Equal(7, res.Total);
            Assert.Equal("Author", res.First().Name);
            Assert.Equal("create", res.First().Policies.First().Actions[0]);
            Assert.Equal("Translator 1", res.ElementAt(4).Name);
            Assert.Equal("Entry", ((res.ElementAt(4).Policies.First().Constraint as AndConstraint).First() as EqualsConstraint).ValueToEqual);

        }

        [Fact]
        public async Task SnapshotsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SnapshotsCollection.json");

            //Act
            var res = await _client.GetAllSnapshotsForEntryAsync("123");

            //Assert
            Assert.Equal("Seven Tips From Ernest Hemingway on How to Write Fiction", res.First().Fields["title"]["en-US"]);
        }

        [Fact]
        public async Task SpaceMembershipsShouldDeserializeCorrectly()
        {
            //Arrange
            _handler.Response = GetResponseFromFile(@"JsonFiles\SampleSpaceMembershipsCollection.json");

            //Act
            var res = await _client.GetSpaceMembershipsAsync();

            //Assert
            Assert.Equal(1, res.Total);
            Assert.True(res.First().Admin);
            Assert.Equal("123", res.First().Roles[1]);
        }

        [Fact]
        public async Task SpaceMembershipShouldSerializeCorrectly()
        {
            //Arrange
            var membership = new SpaceMembership();
            membership.Admin = true;
            membership.Roles = new List<string>()
            {
                "123",
                "231",
                "12344"
            };
            var serializedMembership = JsonConvert.SerializeObject(membership);
            _handler.Response = new HttpResponseMessage() { Content = new StringContent(serializedMembership) };

            //Act
            var res = await _client.CreateSpaceMembershipAsync(membership);

            //Assert
            Assert.True(res.Admin);
            Assert.Equal(3, res.Roles.Count);
            Assert.Equal("231", res.Roles[1]);
        }

        private HttpResponseMessage GetResponseFromFile(string file)
        {
            //So, this is an ugly hack... Any better way to get the absolute path of the test project?
            var projectPath = Directory.GetParent(typeof(Asset).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName;
            var response = new HttpResponseMessage();
            var fullPath = Path.Combine(projectPath, file);
            response.Content = new StringContent(System.IO.File.ReadAllText(fullPath));
            return response;
        }
    }
}
