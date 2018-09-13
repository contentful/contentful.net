using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Xunit;
using Contentful.Core.Models;

namespace Contentful.Core.Tests.Search
{
    public class QueryBuilderTests
    {
        [Fact]
        public void NewUnusedQueryBuilderShouldReturnEmptyString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AddingMultipleQueryParametersShouldCorrectlyAddQueryStringValues()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.ContentTypeIs("123").Include(5).Limit(6).Build();
            //Assert
            Assert.Equal("?content_type=123&include=5&limit=6", result);
        }

        [Fact]
        public void ContentTypeRestrictionShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.ContentTypeIs("123").Build();
            //Assert
            Assert.Equal("?content_type=123", result);
        }

        [Fact]
        public void FullTextSearchShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FullTextSearch("something").Build();
            //Assert
            Assert.Equal("?query=something", result);
        }

        [Fact]
        public void FullTextSearchShouldNotAddQueryStringForShortQuery()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FullTextSearch("s").Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void InProximityOfShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.InProximityOf("locField", "12,-13").Build();
            //Assert
            Assert.Equal("?locField[near]=12%2C-13", result);
        }

        [Fact]
        public void InProximityOfGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.InProximityOf(c => c.Title, "45.45,-13").Build();
            //Assert
            Assert.Equal("?fields.title[near]=45.45%2C-13", result);
        }

        [Fact]
        public void WithinAreaShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.WithinArea("locField", "42", "32","54","-38").Build();
            //Assert
            Assert.Equal("?locField[within]=42%2C32%2C54%2C-38", result);
        }

        [Fact]
        public void WithinAreaGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.WithinArea(c => c.Title, "42", "32", "54", "-38").Build();
            //Assert
            Assert.Equal("?fields.title[within]=42%2C32%2C54%2C-38", result);
        }

        [Fact]
        public void WithinRadiusShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.WithinRadius("locField", "13", "-44", 5).Build();
            //Assert
            Assert.Equal("?locField[within]=13%2C-44%2C5", result);
        }

        [Fact]
        public void WithinRadiusGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.WithinRadius(c => c.Title, "42", "32", 5).Build();
            //Assert
            Assert.Equal("?fields.title[within]=42%2C32%2C5", result);
        }

        [Fact]
        public void OrderByShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.OrderBy("somesortexpression").Build();
            //Assert
            Assert.Equal("?order=somesortexpression", result);
        }

        [Fact]
        public void LimitShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.Limit(5).Build();
            //Assert
            Assert.Equal("?limit=5", result);
        }

        [Fact]
        public void SkipShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.Skip(3).Build();
            //Assert
            Assert.Equal("?skip=3", result);
        }

        [Fact]
        public void IncludeShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.Include(3).Build();
            //Assert
            Assert.Equal("?include=3", result);
        }

        [Fact]
        public void LocaleIsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.LocaleIs("en-US").Build();
            //Assert
            Assert.Equal("?locale=en-US", result);
        }

        [Fact]
        public void MimeTypeIsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.MimeTypeIs(MimeTypeRestriction.Archive).Build();
            //Assert
            Assert.Equal("?mimetype_group=archive", result);
        }

        [Fact]
        public void FieldEqualsShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldEquals("someField", "whatever").Build();
            //Assert
            Assert.Equal("?someField=whatever", result);
        }

        [Fact]
        public void FieldEqualsGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.Name, "whatever").Build();

            //Assert
            Assert.Equal("?fields.name=whatever", result);
        }

        [Fact]
        public void FieldEqualsGenericShouldAddCorrectQueryStringForSysProperty()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.SystemProperties.Id, "123").Build();

            //Assert
            Assert.Equal("?sys.id=123", result);
        }

        [Fact]
        public void FieldEqualsGenericShouldAddCorrectQueryStringForPropertyWithAttribute()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.LongThing, "bob").Build();

            //Assert
            Assert.Equal("?fields.long=bob", result);
        }

        [Fact]
        public void FieldEqualsGenericShouldAddCorrectQueryStringForNestedSysProperty()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.SystemProperties.ContentType.SystemProperties.Id, "123").Build();

            //Assert
            Assert.Equal("?sys.contentType.sys.id=123", result);
        }

        [Fact]
        public void FieldEqualsGenericShouldAddCorrectQueryStringForEntryFieldProperty()
        {
            //Arrange
            var builder = new QueryBuilder<Entry<Author>>();

            //Act
            var result = builder.FieldEquals(c => c.Fields.Name, "bob").Build();

            //Assert
            Assert.Equal("?fields.name=bob", result);
        }

        [Fact]
        public void FieldDoesNotEqualShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldDoesNotEqual("someField", "whatever").Build();
            //Assert
            Assert.Equal("?someField[ne]=whatever", result);
        }

        [Fact]
        public void FieldDoesNotEqualGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldDoesNotEqual(c => c.Name, "whatever").Build();

            //Assert
            Assert.Equal("?fields.name[ne]=whatever", result);
        }

        [Fact]
        public void FieldEqualsAllShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldEqualsAll("someField", new []{"value1", "value2", "andSoOnAndSoForth"}).Build();
            //Assert
            Assert.Equal("?someField[all]=value1%2Cvalue2%2CandSoOnAndSoForth", result);
        }

        [Fact]
        public void FieldEqualsAllGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldEqualsAll(c => c.Title, new[] { "value1", "value2", "andSoOnAndSoForth" }).Build();
            //Assert
            Assert.Equal("?fields.title[all]=value1%2Cvalue2%2CandSoOnAndSoForth", result);
        }

        [Fact]
        public void FieldIncludesShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldIncludes("someField", new[] { "some", "other", "value" }).Build();
            //Assert
            Assert.Equal("?someField[in]=some%2Cother%2Cvalue", result);
        }

        [Fact]
        public void FieldIncludesGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldIncludes(c => c.Title, new[] { "value1", "value2", "andSoOnAndSoForth" }).Build();
            //Assert
            Assert.Equal("?fields.title[in]=value1%2Cvalue2%2CandSoOnAndSoForth", result);
        }

        [Fact]
        public void FieldExcludesShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldExcludes("someField", new[] { "some", "other", "value" }).Build();
            //Assert
            Assert.Equal("?someField[nin]=some%2Cother%2Cvalue", result);
        }

        [Fact]
        public void FieldExcludesGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldExcludes(c => c.Title, new[] { "value1", "value2", "andSoOnAndSoForth" }).Build();
            //Assert
            Assert.Equal("?fields.title[nin]=value1%2Cvalue2%2CandSoOnAndSoForth", result);
        }

        [Theory]
        [InlineData(true, "?fieldOfBeauty[exists]=true")]
        [InlineData(false, "?fieldOfBeauty[exists]=false")]
        public void FieldExistsShouldAddCorrectQueryString(bool mustExist, string expected)
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldExists("fieldOfBeauty", mustExist).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true, "?fields.title[exists]=true")]
        [InlineData(false, "?fields.title[exists]=false")]
        public void FieldExistsGenericShouldAddCorrectQueryString(bool mustExist, string expected)
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldExists(c => c.Title, mustExist).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FieldLessThanShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldLessThan("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[lt]=23", result);
        }

        [Fact]
        public void FieldLessThanGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldLessThan(c => c.Title, "23").Build();
            //Assert
            Assert.Equal("?fields.title[lt]=23", result);
        }

        [Fact]
        public void FieldLessThanOrEqualToShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldLessThanOrEqualTo("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[lte]=23", result);
        }

        [Fact]
        public void FieldLessThanOrEqualToGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldLessThanOrEqualTo(c => c.Title, "23").Build();
            //Assert
            Assert.Equal("?fields.title[lte]=23", result);
        }

        [Fact]
        public void FieldGreaterThanShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldGreaterThan("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[gt]=23", result);
        }

        [Fact]
        public void FieldGreaterThanGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldGreaterThan(c => c.Title, "23").Build();
            //Assert
            Assert.Equal("?fields.title[gt]=23", result);
        }

        [Fact]
        public void FieldGreaterThanOrEqualToShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldGreaterThanOrEqualTo("fieldOfBeauty", "23").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[gte]=23", result);
        }

        [Fact]
        public void FieldGreaterThanOrEqualToGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldGreaterThanOrEqualTo(c => c.Title, "23").Build();
            //Assert
            Assert.Equal("?fields.title[gte]=23", result);
        }

        [Fact]
        public void FieldMatchesShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<object>();
            //Act
            var result = builder.FieldMatches("fieldOfBeauty", "matchMe!").Build();
            //Assert
            Assert.Equal("?fieldOfBeauty[match]=matchMe!", result);
        }

        [Fact]
        public void FieldMatchesGenericShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<TestModelWithIncludes>();
            //Act
            var result = builder.FieldMatches(c => c.Title, "matchMe").Build();
            //Assert
            Assert.Equal("?fields.title[match]=matchMe", result);
        }

        [Fact]
        public void FieldEqualsForJsonHandledPropertyShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.NotCamel, "whatever").Build();

            //Assert
            Assert.Equal("?fields.NotCamelISay=whatever", result);
        }

        [Fact]
        public void FieldEqualsForJsonHandledPropertyByNameShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.NotACamelEither, "whatever").Build();

            //Assert
            Assert.Equal("?fields.NoCamelHere=whatever", result);
        }

        [Fact]
        public void FieldEqualsForIncomingEntryLinksShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.LinksToEntry("some-id").Build();

            //Assert
            Assert.Equal("?links_to_entry=some-id", result);
        }

        [Fact]
        public void FieldEqualsForIncomingAssetLinksShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.LinksToAsset("some-id").Build();

            //Assert
            Assert.Equal("?links_to_asset=some-id", result);
        }

        [Fact]
        public void FieldEqualsForSubreferenceFieldShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.Test.Field1, "something").Build();

            //Assert
            Assert.Equal("?fields.test.fields.field1=something", result);
        }

        [Fact]
        public void FieldEqualsForSubreferenceInMultipleLevelsFieldShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.Test.Cat.Title, "bur").Build();

            //Assert
            Assert.Equal("?fields.test.fields.cat.fields.title=bur", result);
        }

        [Fact]
        public void FieldEqualsForSubreferenceFieldAndContentTypeShouldAddCorrectQueryString()
        {
            //Arrange
            var builder = new QueryBuilder<Author>();

            //Act
            var result = builder.FieldEquals(c => c.Test.Sys.ContentType.SystemProperties.Id, "123").FieldEquals(c => c.Test.Field1, "something").Build();

            //Assert
            Assert.Equal("?fields.test.sys.contentType.sys.id=123&fields.test.fields.field1=something", result);
        }
    }
}
