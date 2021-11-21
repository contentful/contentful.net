using Contentful.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests.Extensions
{
    public class JsonConversionExtensionsTests
    {
        [Fact]
        public void JsonConversionExtensionsWorksSimpleObject()
        {
            // Arrange
            var obj = new
            {
                foo = "bar"
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", convertedString);
        }

        [Fact]
        public void JsonConversionExtensionsWorksNestedObject()
        {
            // Arrange
            var obj = new
            {
                foo = "bar",
                bar = new
                {
                    baz = "qux"
                }
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"foo\":\"bar\",\"bar\":{\"baz\":\"qux\"}}", convertedString);
        }

        [Fact]
        public void JsonConversionExtensionsWorksArray()
        {
            // Arrange
            var obj = new
            {
                foo = "bar",
                bar = new
                {
                    baz = "qux"
                },
                baz = new[] {
              new {
                foo = "bar"
              },
              new {
                foo = "baz"
              }
            }
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"foo\":\"bar\",\"bar\":{\"baz\":\"qux\"},\"baz\":[{\"foo\":\"bar\"},{\"foo\":\"baz\"}]}", convertedString);
        }

        [Fact]
        public void JsonConversionExtensionsWorksArrayWithNull()
        {
            // Arrange
            var obj = new
            {
                foo = "bar",
                bar = new
                {
                    baz = "qux"
                },
                baz = new[] {
              new {
                foo = "bar"
              },
              null
            }
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"foo\":\"bar\",\"bar\":{\"baz\":\"qux\"},\"baz\":[{\"foo\":\"bar\"},null]}", convertedString);
        }

        [Fact]
        public void JsonConversionExtensionsConvertsToCamlCase()
        {
            // Arrange
            var obj = new
            {
                ThisIsATest = "bar",
                ThisIsAnotherTest = "baz",
                this_is_a_test_snake = "qux"
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"thisIsATest\":\"bar\",\"thisIsAnotherTest\":\"baz\",\"this_is_a_test_snake\":\"qux\"}", convertedString);
        }

        [Fact]
        public void JsonConversionExtensionsConvertsEmargoedAssetBodyCorrectly()
        {
            // Arrange
            var obj = new
            {
                ExpiresAt = new DateTimeOffset(2021, 5, 12, 3, 4, 5, TimeSpan.Zero).ToUnixTimeMilliseconds()
            };

            // Act
            var convertedString = obj.ConvertObjectToJsonString();

            // Assert
            Assert.Equal("{\"expiresAt\":1620788645000}", convertedString);

        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(13, 13)]
        [InlineData(896, 896)]
        [InlineData(-345, -345)]
        public void ParsingNullableIntValuesShouldYieldCorrectResult(int? val, int? exptected)
        {
            //Arrange
            var token = new JObject(new JProperty("val", val))["val"];

            //Act
            var res = token.ToNullableInt();

            //Assert
            Assert.Equal(exptected, res);
        }
    }
}
