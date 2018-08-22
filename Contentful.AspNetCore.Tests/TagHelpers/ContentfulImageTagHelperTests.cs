using Contentful.AspNetCore.TagHelpers;
using Contentful.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Contentful.AspNetCore.Tests.TagHelpers
{
    public class ContentfulImageTagHelperTests
    {
        [Fact]
        public async Task AltTextShouldBeSetIfMissingAndDescriptionIsPresent()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se"
                }
            };

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(),  (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("Banana", output.Attributes["alt"].Value);
        }

        [Fact]
        public async Task AltTextShouldNotBeOverWrittenIfSet()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se"
                }
            };

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList() { new TagHelperAttribute("alt", "alt text") }, (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("alt text", output.Attributes["alt"].Value);
        }

        [Fact]
        public async Task SrcSetShouldBePresentIfThereAreSources()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se"
                }
            };

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(), async (b, c) => {

                var childSource = new ContentfulSource(clientMock.Object);
                childSource.Width = 500;
                await childSource.ProcessAsync(context, new TagHelperOutput("div", new TagHelperAttributeList(), (x,y) => { return null; }));
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return tagHelperContent;
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("https://robertlinde.se?w=500 500w", output.Attributes["srcset"].Value);
        }

        [Fact]
        public async Task SourceShouldSetDefaultSizeIfNotSpecified()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulSource(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se"
                }
            };

            tagHelper.Width = 500;

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");

            context.Items.Add("sources", new List<string>());
            context.Items.Add("defaults", null);

            var output = new TagHelperOutput("div", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("500w", tagHelper.Size);
        }

        [Fact]
        public async Task SourceSizeShouldNotBeOverriddenWhenSet()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulSource(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se"
                }
            };

            tagHelper.Width = 500;
            tagHelper.Size = "200w";

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");

            context.Items.Add("sources", new List<string>());
            context.Items.Add("defaults", null);

            var output = new TagHelperOutput("div", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("200w", tagHelper.Size);
        }

        [Fact]
        public async Task JpgSpecificValuesShouldNotBeSetForNonJpgAsset()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se",
                    ContentType = "image/png"
                }
            };

            tagHelper.JpgQuality = 45;
            tagHelper.ProgressiveJpg = true;
            tagHelper.Width = 50;

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("https://robertlinde.se?w=50", output.Attributes["src"].Value);
        }

        [Fact]
        public async Task JpgSpecificValuesShouldNotBeSetForNonJpgAssetWithFormat()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se",
                    ContentType = "image/jpeg"
                }
            };
            tagHelper.Format = Core.Images.ImageFormat.Png;
            tagHelper.JpgQuality = 45;
            tagHelper.ProgressiveJpg = true;
            tagHelper.Width = 50;

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("https://robertlinde.se?w=50&fm=png", output.Attributes["src"].Value);
        }

        [Fact]
        public async Task JpgSpecificValuesShouldBeSetForJpgAsset()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se",
                    ContentType = "image/jpeg"
                }
            };

            tagHelper.JpgQuality = 45;
            tagHelper.ProgressiveJpg = true;
            tagHelper.Width = 50;

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("https://robertlinde.se?w=50&q=45&fl=progressive", output.Attributes["src"].Value);
        }

        [Fact]
        public async Task JpgSpecificValuesShouldBeSetForJpgAssetWithFormat()
        {
            //Arrange
            var clientMock = new Mock<IContentfulClient>();
            var tagHelper = new ContentfulImageTagHelper(clientMock.Object);
            tagHelper.Asset = new Core.Models.Asset()
            {
                Description = "Banana",
                File = new Core.Models.File()
                {
                    Url = "https://robertlinde.se",
                    ContentType = "image/png"
                }
            };

            tagHelper.JpgQuality = 45;
            tagHelper.ProgressiveJpg = true;
            tagHelper.Format = Core.Images.ImageFormat.Jpg;
            tagHelper.Width = 50;

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("img", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await tagHelper.ProcessAsync(context, output);

            //Assert
            Assert.Equal("https://robertlinde.se", tagHelper.Url);
            Assert.Equal("https://robertlinde.se?w=50&q=45&fm=jpg&fl=progressive", output.Attributes["src"].Value);
        }
    }
}
