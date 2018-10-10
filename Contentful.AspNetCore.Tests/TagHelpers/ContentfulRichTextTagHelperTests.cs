using Contentful.AspNetCore.TagHelpers;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Contentful.AspNetCore.Tests.TagHelpers
{
    public class ContentfulRichTextTagHelperTests
    {
        [Fact]
        public async Task RichTextTagHelperShouldGenerateCorrectHTmlOutput()
        {
            //Arrange
            var taghelper = new ContentfulRichTextTagHelper(new HtmlRenderer());
            taghelper.Document = new Document
            {
                Content = new List<IContent>
                {
                    new Paragraph
                    {
                        Content = new List<IContent>
                        {
                            new Text
                            {
                                Value = "Hello friends!"
                            }
                        }
                    }
                }
            };

            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("div", new TagHelperAttributeList(), (b, c) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            //Act
            await taghelper.ProcessAsync(context, output);
            var html = output.Content.GetContent();
            //Assert
            Assert.Equal("<p>Hello friends!</p>", html);
        }
    }
}
