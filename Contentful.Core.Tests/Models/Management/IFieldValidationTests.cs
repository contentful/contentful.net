using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests.Models.Management
{
    public class IFieldValidationTests
    {
        [Theory]
        [InlineData("This is the message", "123", "666")]
        [InlineData("Another message", "765", "988")]
        [InlineData("A final message", "AnId", "AndAnother")]
        public void LinkContentTypeValidatorShouldReturnCorrectJson(string message, string id, string id2)
        {
            //Arrange
            var validator = new LinkContentTypeValidator(message, id, id2);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""linkContentType"":[""{id}"",""{id2}""],""message"":""{message}""}}", json);
        }

        [Theory]
        [InlineData("This is the message")]
        [InlineData("Another message")]
        [InlineData("A final message")]
        public void LinkContentTypeValidatorShouldReturnCorrectJsonForContentTypes(string message)
        {
            //Arrange
            var cTypes = new List<ContentType>()
            {
                new ContentType { SystemProperties = new SystemProperties { Id="557" } },
                new ContentType { SystemProperties = new SystemProperties { Id="379" } }
            };
            var validator = new LinkContentTypeValidator(cTypes, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""linkContentType"":[""557"",""379""],""message"":""{message}""}}", json);
        }

        [Fact]
        public void UniqueValidatorShouldReturnCorrectJSon()
        {
            //Arrange
            var validator = new UniqueValidator();
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal(@"{""unique"":true}", json);
        }

        [Theory]
        [InlineData("This is the message", "value1", "value2")]
        [InlineData("Another message", "bob", "alice")]
        [InlineData("A final message", "Crag", "Hack")]
        public void InValuesValidatorShouldReturnCorrectJson(string message, string val, string val2)
        {
            //Arrange
            var validator = new InValuesValidator(message, val, val2);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""in"":[""{val}"",""{val2}""],""message"":""{message}""}}", json);
        }

        [Theory]
        [InlineData("This is the message", MimeTypeRestriction.Archive, MimeTypeRestriction.Image)]
        [InlineData("Another message", MimeTypeRestriction.Markup, MimeTypeRestriction.Plaintext)]
        [InlineData("A final message", MimeTypeRestriction.Spreadsheet, MimeTypeRestriction.Richtext)]
        public void MimeTypesValidatorShouldReturnCorrectJson(string message, MimeTypeRestriction val, MimeTypeRestriction val2)
        {
            //Arrange
            var validator = new MimeTypeValidator(new[] { val, val2 }, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""linkMimetypeGroup"":[""{val.ToString().ToLower()}"",""{val2.ToString().ToLower()}""],""message"":""{message}""}}", json);
        }

        [Theory]
        [InlineData("This is the message", 1, 8)]
        [InlineData("Another message", 3, 23)]
        [InlineData("A final message", 11, 65)]
        public void RangeValidatorShouldReturnCorrectJson(string message, int min, int max)
        {
            //Arrange
            var validator = new RangeValidator(min, max, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""range"":{{""min"":{min},""max"":{max}}},""message"":""{message}""}}", json);
        }

        [Theory]
        [InlineData("This is the message", "2016-01-01", "2017-01-22")]
        [InlineData("Another message", "2012-03-05", "2017-01-03")]
        [InlineData("A final message", "2017-08-06", "2030-11-25")]
        public void DateRangeValidatorShouldReturnCorrectJson(string message, string min, string max)
        {
            //Arrange
            var validator = new DateRangeValidator(min, max, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""dateRange"":{{""min"":""{min}"",""max"":""{max}""}},""message"":""{message}""}}", json);
        }

        [Fact]
        public void DateRangeValidatorShouldReturnCorrectJsonForNullValues()
        {
            //Arrange
            var validator = new DateRangeValidator(null, null, "Bobs message");
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""dateRange"":{{""min"":null,""max"":null}},""message"":""Bobs message""}}", json);
        }

        [Fact]
        public void DateRangeValidatorShouldReturnCorrectDateValues()
        {
            //Arrange
            var validator = new DateRangeValidator("2012-01-03", "2018-01-04", "Bobs message");
            //Act
            var min = validator.Min;
            var max = validator.Max;
            //Assert
            Assert.Equal(min.Value, DateTime.Parse("2012-01-03"));
            Assert.Equal(max.Value, DateTime.Parse("2018-01-04"));
        }

        [Fact]
        public void DateRangeValidatorShouldReturnNullForIncorrectDateValues()
        {
            //Arrange
            var validator = new DateRangeValidator("Gibberish", "Banana", "Bobs message");
            //Act
            var min = validator.Min;
            var max = validator.Max;
            //Assert
            Assert.Null(min);
            Assert.Null(max);
        }

        [Theory]
        [InlineData("This is the message", 25, 37, 14, 34)]
        [InlineData("Another message", 100, 150, 120, 180)]
        [InlineData("A final message", 1024, 2048, 768, 1536)]
        public void ImageSizeValidatorShouldReturnCorrectJson(string message, int minWidth, int maxWidth, int minHeight, int maxHeight)
        {
            //Arrange
            var validator = new ImageSizeValidator(minWidth, maxWidth, minHeight, maxHeight, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""assetImageDimensions"":{{""width"":{{""min"":{minWidth},""max"":{maxWidth}}},""height"":{{""min"":{minHeight},""max"":{maxHeight}}}}},""message"":""{message}""}}", json);
        }

        [Theory]
        [InlineData("This is the message", 25, 37, SystemFileSizeUnits.Bytes, SystemFileSizeUnits.Bytes, 25, 37)]
        [InlineData("Another message", 1, 1, SystemFileSizeUnits.KB, SystemFileSizeUnits.MB, 1024, 1048576)]
        [InlineData("A final message", 1, 100, SystemFileSizeUnits.MB, SystemFileSizeUnits.MB, 1048576, 104857600)]
        public void FileSizeValidatorShouldReturnCorrectJson(string message, int min, int max, string minUnit, string maxUnit, int expectedMin, int expectedMax)
        {
            //Arrange
            var validator = new FileSizeValidator(min, max, minUnit, maxUnit, message);
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""assetFileSize"":{{""min"":{expectedMin},""max"":{expectedMax}}},""message"":""{message}""}}", json);
        }

        [Fact]
        public void NodesValidatorShouldReturnCorrectJson()
        {
            //Arrange
            var validator = new NodesValidator();
            validator.EmbeddedEntryBlock = new List<IFieldValidator>() {
                new SizeValidator(2,4)
            };
            validator.EmbeddedEntryInline = new List<IFieldValidator>() {
                new SizeValidator(8, null)
            };
            validator.EntryHyperlink = new List<IFieldValidator>() {
                new SizeValidator(null, 3)
            };
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal(@"{""nodes"":{""entry-hyperlink"":[{""size"":{""min"":null,""max"":3},""message"":null}],""embedded-entry-block"":[{""size"":{""min"":2,""max"":4},""message"":null}],""embedded-entry-inline"":[{""size"":{""min"":8,""max"":null},""message"":null}]}}", json);
        }

        [Theory]
        [InlineData(new[] { SystemNodeTypes.BLOCKS_HEADING_2, 
            SystemNodeTypes.BLOCKS_HEADING_3, SystemNodeTypes.BLOCKS_HEADING_4 }, 
            null,
            @"{""enabledNodeTypes"":[""heading-2"",""heading-3"",""heading-4""],""message"":null}")]
        [InlineData(new[] { SystemNodeTypes.BLOCKS_EMBEDDED_ASSET,
            SystemNodeTypes.BLOCKS_EMBEDDED_ENTRY, SystemNodeTypes.BLOCKS_HR },
            "massage-message",
            @"{""enabledNodeTypes"":[""embedded-asset-block"",""embedded-entry-block"",""hr""],""message"":""massage-message""}")]
        public void EnabledNodeTypesValidatorShouldReturnCorrectJson(IEnumerable<string> vals, string message, string expected)
        {
            //Arrange
            var validator = new EnabledNodeTypesValidator(vals, message);

            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);

            //Assert
            Assert.Equal(expected, json);
        }

        [Fact]
        public void ProhibitRegexValidatorShouldReturnCorrectJson()
        {
            //Arrange
            var validator = new ProhibitRegexValidator("foo", "g", "Babas message");
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""prohibitRegexp"":{{""pattern"":""foo"",""flags"":""g""}},""message"":""Babas message""}}", json);
        }

        [Fact]
        public void RegexValidatorShouldReturnCorrectJson()
        {
            //Arrange
            var validator = new RegexValidator("foo", "g", "Babas message");
            //Act
            var created = validator.CreateValidator();
            var json = JsonConvert.SerializeObject(created);
            //Assert
            Assert.Equal($@"{{""regexp"":{{""pattern"":""foo"",""flags"":""g""}},""message"":""Babas message""}}", json);
        }
    }
}
