using Contentful.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests
{
    public class ContentfulManagementClientTests
    {
        private ContentfulManagementClient _client;
        private FakeMessageHandler _handler;

        public ContentfulManagementClientTests()
        {

            _handler = new FakeMessageHandler();
            
        }

        [Fact]
        public void CreatingManagementClientShouldSetHeadersCorrectly()
        {
            //Arrange
            var httpClient = new HttpClient(_handler);
            _client = new ContentfulManagementClient(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = "123",
                ManagementApiKey = "564",
                SpaceId = "666",
                UsePreviewApi = false
            });
            //Act
            //Assert
            Assert.Equal("564", httpClient.DefaultRequestHeaders.Authorization.Parameter);
        } 
    }
}
