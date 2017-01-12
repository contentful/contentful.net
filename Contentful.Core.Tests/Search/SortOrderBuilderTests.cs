using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Xunit;

namespace Contentful.Core.Tests.Search
{
    public class SortOrderBuilderTests
    {
        [Fact]
        public void CreatedSortOrderBuilderShouldHoldOnlyOneSortExpressionWithDefaultSorting()
        {
            //Arrange
            var builder = SortOrderBuilder<object>.New("sys.created");
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("sys.created", res);
        }

        [Fact]
        public void CreatedSortOrderBuilderGenericShouldHoldOnlyOneSortExpressionWithDefaultSorting()
        {
            //Arrange
            var builder = SortOrderBuilder<Author>.New(a => a.Name);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("fields.name", res);
        }

        [Fact]
        public void CreatedSortOrderBuilderWithReversedOrderShouldHoldOnlyOneSortExpressionWithReversedSorting()
        {
            //Arrange
            var builder = SortOrderBuilder<object>.New("sys.created", SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-sys.created", res);
        }

        [Fact]
        public void CreatedSortOrderBuilderGenericWithReversedOrderShouldHoldOnlyOneSortExpressionWithReversedSorting()
        {
            //Arrange
            var builder = SortOrderBuilder<Author>.New(a => a.Name, SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-fields.name", res);
        }

        [Fact]
        public void AddingSortOrderParametersShouldYieldCorrectSortExpression()
        {
            //Arrange
            var builder =
                SortOrderBuilder<object>.New("sys.created", SortOrder.Reversed).ThenBy("field.name")
                    .ThenBy("field.date", SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-sys.created,field.name,-field.date", res);
        }

        [Fact]
        public void AddingSortOrderParametersGenericShouldYieldCorrectSortExpression()
        {
            //Arrange
            var builder =
                SortOrderBuilder<Author>.New(a => a.SystemProperties.Id, SortOrder.Reversed).ThenBy(a => a.Name)
                    .ThenBy(a => a.LongThing, SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-sys.id,fields.name,-fields.long", res);
        }
    }
}
