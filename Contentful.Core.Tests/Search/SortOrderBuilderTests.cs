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
            var builder = new SortOrderBuilder("sys.created");
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("sys.created", res);
        }

        [Fact]
        public void CreatedSortOrderBuilderWithReversedOrderShouldHoldOnlyOneSortExpressionWithReversedSorting()
        {
            //Arrange
            var builder = new SortOrderBuilder("sys.created", SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-sys.created", res);
        }

        public void AddingSortOrderParametersShouldYieldCorrectSortExpression()
        {
            //Arrange
            var builder =
                new SortOrderBuilder("sys.created", SortOrder.Reversed).ThenBy("field.name")
                    .ThenBy("field.date", SortOrder.Reversed);
            //Act
            var res = builder.Build();
            //Assert
            Assert.Equal("-sys.created,field.name,-field.date", res);
        }
    }
}
