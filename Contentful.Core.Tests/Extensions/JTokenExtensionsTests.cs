using Contentful.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests.Extensions
{
    public class JTokenExtensionsTests
    {
        [Fact]
        public void JTokenIsNullShouldReturnTrueForNullValue()
        {
            //Arrange
            JToken token = null;

            //Act
            var res = token.IsNull();

            //Assert
            Assert.True(res);
        }

        [Fact]
        public void JTokenIsNullShouldReturnTrueForNullType()
        {
            //Arrange
            string json = @"
                {
                  'test': null
                }";

            //Act
            var jObject = JObject.Parse(json);
            var res = jObject["test"].IsNull();

            //Assert
            Assert.True(res);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(13, 13)]
        [InlineData(896, 896)]
        [InlineData(-345, -345)]
        public void ParsingIntValuesShouldYieldCorrectResult(int? val, int exptected)
        {
            //Arrange
            var token = new JObject(new JProperty("val", val))["val"];
            
            //Act
            var res = token.ToInt();

            //Assert
            Assert.Equal(exptected, res);
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
