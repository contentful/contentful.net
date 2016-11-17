using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Xunit;

namespace Contentful.Core.Tests.Search
{
    public class QueryBuilderTests
    {
        [Fact]
        public void NewUnusedQueryBuilderShouldReturnEmptyString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AddingMultipleQueryParametersShouldCorrectlyAddQueryStringValues()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.ContentTypeIs("123").Include(5).Limit(6).Build();
            //Assert
            Assert.Equal("?content_type=123&include=5&limit=6", result);
        }

        [Fact]
        public void ContentTypeRestrictionShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.ContentTypeIs("123").Build();
            //Assert
            Assert.Equal("?content_type=123", result);
        }

        [Fact]
        public void FullTextSearchShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FullTextSearch("something").Build();
            //Assert
            Assert.Equal("?query=something", result);
        }

        [Fact]
        public void FullTextSearchShouldNotAddQueryStringForShortQuery()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FullTextSearch("s").Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void InProximityOfShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.InProximityOf("locField", "12,-13").Build();
            //Assert
            Assert.Equal("?locField[near]=12,-13", result);
        }

        [Fact]
        public void WithinAreaShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.WithinArea("locField", "42", "32","54","-38").Build();
            //Assert
            Assert.Equal("?locField[within]=42,32,54,-38", result);
        }

        [Fact]
        public void WithinRadiusShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.WithinRadius("locField", "13", "-44", 5).Build();
            //Assert
            Assert.Equal("?locField[within]=13,-44,5", result);
        }

        [Fact]
        public void OrderByShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.OrderBy("somesortexpression").Build();
            //Assert
            Assert.Equal("?order=somesortexpression", result);
        }

        [Fact]
        public void LimitShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.Limit(5).Build();
            //Assert
            Assert.Equal("?limit=5", result);
        }

        [Fact]
        public void SkipShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.Skip(3).Build();
            //Assert
            Assert.Equal("?skip=3", result);
        }

        [Fact]
        public void IncludeShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.Include(3).Build();
            //Assert
            Assert.Equal("?include=3", result);
        }

        [Fact]
        public void LocaleIsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.LocaleIs("en-US").Build();
            //Assert
            Assert.Equal("?locale=en-US", result);
        }

        [Fact]
        public void MimeTypeIsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.MimeTypeIs(MimeTypeRestriction.Archive).Build();
            //Assert
            Assert.Equal("?mimetype_group=archive", result);
        }

        [Fact]
        public void FieldEqualsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldEquals("someField", "whatever").Build();
            //Assert
            Assert.Equal("?someField=whatever", result);
        }

        [Fact]
        public void FieldDoesNotEqualShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldDoesNotEqual("someField", "whatever").Build();
            //Assert
            Assert.Equal("?someField[ne]=whatever", result);
        }

        [Fact]
        public void FieldEqualsAllShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldEqualsAll("someField", new []{"value1", "value2", "andSoOnAndSoForth"}).Build();
            //Assert
            Assert.Equal("?someField[all]=value1,value2,andSoOnAndSoForth", result);
        }

        [Fact]
        public void FieldIncludesShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldIncludes("someField", new[] { "some", "other", "value" }).Build();
            //Assert
            Assert.Equal("?someField[in]=some,other,value", result);
        }

        [Fact]
        public void FieldExcludesShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldExcludes("someField", new[] { "some", "other", "value" }).Build();
            //Assert
            Assert.Equal("?someField[nin]=some,other,value", result);
        }

        [Fact]
        public void FieldExistsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldExists("fieldOfBeauty").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[exists]=true", result);
        }

        [Fact]
        public void FieldLessThanShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldLessThan("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[lt]=23", result);
        }

        [Fact]
        public void FieldLessThanOrEqualToShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldLessThanOrEqualTo("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[lte]=23", result);
        }

        [Fact]
        public void FieldGreaterThanShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldGreaterThan("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[gt]=23", result);
        }

        [Fact]
        public void FieldGreaterThanOrEqualToShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldGreaterThanOrEqualTo("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[gte]=23", result);
        }

        [Fact]
        public void FieldMatchesToShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder();
            //Act
            var result = builder.FieldMatches("fieldOfBeauty", "matchMe!").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[match]=matchMe!", result);
        }
    }
}
