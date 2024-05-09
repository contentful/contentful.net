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

        [Fact]
        public async Task MarksShouldRenderToCorrectHTmlOutput()
        {
            //Arrange
            var renderer = new HtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                    {
                        new Paragraph
                        {
                            Content = new List<IContent>
                            {
                                new Text
                                {
                                    Value = "Hello friends!",
                                    Marks = new List<Mark>
                                    {
                                        new Mark
                                        {
                                            Type = "bold"
                                        },
                                        new Mark
                                        {
                                            Type = "italic"
                                        }
                                    }
                                }
                            }
                        }
                    }
            };

            //Act
            var html = await renderer.ToHtml(doc);
            //Assert
            Assert.Equal("<p><strong><em>Hello friends!</em></strong></p>", html);
        }

        [Fact]
        public async Task TextShouldRenderEscapedToCorrectHTmlOutput()
        {
            //Arrange
            var renderer = new HtmlRenderer();
            var doc = new Document
            {
                Content = new List<IContent>
                    {
                        new Paragraph
                        {
                            Content = new List<IContent>
                            {
                                new Text
                                {
                                    Value = "Hello friends!<script>alert(0)</script>",
                                    Marks = new List<Mark>
                                    {
                                        new Mark
                                        {
                                            Type = "bold"
                                        },
                                        new Mark
                                        {
                                            Type = "italic"
                                        }
                                    }
                                }
                            }
                        }
                    }
            };

            //Act
            var html = await renderer.ToHtml(doc);
            //Assert
            Assert.Equal("<p><strong><em>Hello friends!&lt;script&gt;alert(0)&lt;/script&gt;</em></strong></p>", html);
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

        [Fact]
        public async Task AssetShouldRenderEmptyLinkWithNoFile()
        {
            //Arrange
            var renderer = new AssetRenderer(null);
            var assetStructure = new AssetStructure
            {
                Content = new List<IContent>(),
                Data = new AssetStructureData
                {
                    Target = new Asset()
                },
                NodeType = ""
            };

            //Act
            var html = await renderer.RenderAsync(assetStructure);
            //Assert
            Assert.Equal("<a></a>", html);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task TableCellShouldBeRenderedWithoutColspanIfColspanNotGreaterThan1(int? colspan)
        {
            //Arrange
            var renderer = new TableCellRenderer(null);
            var tableCell = new TableCell
            {
                Content = new List<IContent>(),
                Data = new TableCellData
                {
                    Colspan = colspan
                },
                NodeType = string.Empty
            };

            //Act
            var html = await renderer.RenderAsync(tableCell);

            //Assert
            Assert.Equal("<td></td>", html);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task TableCellShouldBeRenderedWithoutRowspanIfColspanNotGreaterThan1(int? rowspan)
        {
            //Arrange
            var renderer = new TableCellRenderer(null);
            var tableCell = new TableCell
            {
                Content = new List<IContent>(),
                Data = new TableCellData
                {
                    Rowspan = rowspan
                },
                NodeType = string.Empty
            };

            //Act
            var html = await renderer.RenderAsync(tableCell);

            //Assert
            Assert.Equal("<td></td>", html);
        }

        [Theory]
        [InlineData(2, 3, "<td rowspan=\"2\" colspan=\"3\"></td>")]
        [InlineData(3, null, "<td rowspan=\"3\"></td>")]
        [InlineData(1, 3, "<td colspan=\"3\"></td>")]
        public async Task TableCellShouldBeRenderedWithCorrectRowspanAndColspan(int? rowspan, int? colspan, string expected)
        {
            //Arrange
            var renderer = new TableCellRenderer(null);
            var tableCell = new TableCell
            {
                Content = new List<IContent>(),
                Data = new TableCellData
                {
                    Rowspan = rowspan,
                    Colspan = colspan
                },
                NodeType = string.Empty
            };

            //Act
            var html = await renderer.RenderAsync(tableCell);

            //Assert
            Assert.Equal(expected, html);
        }
    }
}
