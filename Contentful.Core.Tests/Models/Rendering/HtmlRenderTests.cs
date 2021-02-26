using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace Contentful.Core.Tests.Models.Rendering
{
    public class HtmlRenderTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public HtmlRenderTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task RenderingNullOrEmptyContentShouldNotThrowAnException()
        {
            // Arrange
            var renderer = new HtmlRenderer();
            var emptyDocument = new Document();
            
            //Act
            var nullResult = await renderer.ToHtml(null);
            var emptyResult = await renderer.ToHtml(emptyDocument);
            
            // Assert
            Assert.Equal(string.Empty, nullResult);
            Assert.Equal(string.Empty, emptyResult);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ListItemRendererShouldRespectRendererOptions(bool omitParagraphsInsideListItems)
        {
            //Arrange
            var expectedResult = $"<ul><li>{(omitParagraphsInsideListItems ? "" : "<p>")}testing{(omitParagraphsInsideListItems ? "" : "</p>")}</li></ul>";
            _testOutputHelper.WriteLine($"Expected result: {expectedResult}");
            var rendererOptions = new HtmlRendererOptions
            {
                ListItemOptions = new ListItemContentRendererOptions{ OmitParagraphTagsInsideListItems = omitParagraphsInsideListItems }
            };
            var renderer = new HtmlRenderer(rendererOptions);
            var doc = new Document();
            var list = new List();
            var listItem = new ListItem();
            var paragraph = new Paragraph();
            var text = new Text();
            text.Value = "testing";
            paragraph.Content = new List<IContent> { text };
            listItem.Content = new List<IContent> { paragraph };
            list.Content = new List<IContent> { listItem };
            doc.Content = new List<IContent> { list };
            
            //Act
            var result = await renderer.ToHtml(doc);
            _testOutputHelper.WriteLine($"Actual result: {result}");
            
            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}