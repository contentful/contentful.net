using Contentful.Core.Models;
using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Contentful.Core.Tests.Extensions.StringContentHelpers;

namespace Contentful.Core.Tests.Search
{
    public class SelectVisitorTests
    {
        [Fact]
        public void Visitor_GetsField_HasAttributeRename_ShowsInQuery()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntryModel>();

            // Act
            var qry = builder.SelectFields(x => new { x.SystemMetadata }).Build();

            // Assert
            Assert.Equal($"?{Encoded("select", "fields.$metadata")}", qry);
        }

        [Fact]
        public void Visitor_GetsSysFields_DeeperThanImmediate_Throws()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var testDelegate = () => { builder.SelectFields(x => new { x.Title }, x => new { x.ContentType.Name }); };

            // Assert
            Assert.Throws<ArgumentException>(testDelegate);
        }

        [Fact]
        public void Visitor_GetsRegularFields_DeeperThanImmediate_Throws()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var callback = () => { builder.SelectFields(x => new { x.Title, x.MainImage.TitleLocalized }, true); };

            // Assert
            Assert.Throws<ArgumentException>(callback);
        }

        [Fact]
        public void Visitor_GetsNoFields_IncludesSys_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>().SelectFields<TestEntry>(null, true);

            // Act
            var request = builder.Build();

            // Assert
            Assert.Equal($"?select=sys", request);
        }

        [Fact]
        public void Visitor_EmptyFieldSelection_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var request = builder.SelectFields(x => new { }).ContentTypeIs("testEntry").Build();

            // Assert
            Assert.Equal("?content_type=testEntry", request);
        }

        [Fact]
        public void Visitor_LocalSysProperty_InFieldsSelections_Throws()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var callback = () => { builder.SelectFields(x => new { x.Sys }); };

            // Assert
            Assert.Throws<ArgumentException>(callback);
        }

        [Fact]
        public void Visitor_FieldsWithEmptySysSelector_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var qry = builder.SelectFields(x => new { x.Title, x.Id, x.MainImage }, x => new { }).Build();

            // Assert
            Assert.Equal($"?{Encoded("select", "fields.title,fields.id,fields.mainImage")}", qry);
        }

        [Fact]
        public void Visitor_FieldsWithNullSysSelector_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var qry = builder.SelectFields<object,SystemProperties>(x => new { x.Title, x.Id, x.MainImage }, null).Build();

            // Assert
            Assert.Equal($"?{Encoded("select", "fields.title,fields.id,fields.mainImage")}", qry);
        }

        [Fact]
        public void Visitor_EmptyFieldsWithSysSelector_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var qry = builder.SelectFields(x => new { }, x => new { x.Id, x.Environment }).Build();

            // Assert
            Assert.Equal($"?{Encoded("select", "sys.id,sys.environment")}", qry);
        }

        [Fact]
        public void Visitor_NullFieldsWithSysSelector_Works()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var qry = builder.SelectFields<object, object>(null, x => new { x.Id, x.Environment }).Build();

            // Assert
            Assert.Equal($"?{Encoded("select", "sys.id,sys.environment")}", qry);
        }

        [Fact]
        public void Visitor_NullFieldsWithNullSysSelector_NoSelect()
        {
            // Arrange
            var builder = new QueryBuilder<TestEntry>();

            // Act
            var qry = builder.SelectFields<object, SystemProperties>(null, null).Build();

            // Assert
            Assert.Equal(string.Empty, qry);
        }
    }
}
