using System;
using System.Linq;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Xunit;

namespace Contentful.Core.Tests.Search;

public class ScheduledActionQueryBuilderTest
{
    [Fact]
    public void EnvironmentIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        queryBuilder.EnvironmentIs("master");

        var queryString = queryBuilder.Build();

        Assert.Equal("?environment.sys.id=master", queryString);
    }

    [Fact]
    public void EntityIdIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        string entityId = "123";
        queryBuilder.EntityIdIs(entityId);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?entity.sys.id={entityId}", queryString);
    }

    [Fact]
    public void EntityIdInCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        var entityIds = new[] {"123", "456"};
        queryBuilder.EntityIdIn(entityIds);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?entity.sys.id={System.Net.WebUtility.UrlEncode(string.Join(",", entityIds))}", queryString);
    }

    [Fact]
    public void ActionStatusIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        ScheduledActionStatus status = ScheduledActionStatus.Scheduled;
        queryBuilder.StatusIs(status);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?sys.status={status.ToString().ToLower()}", queryString);
    }

    [Fact]
    public void ActionStatusInCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        var statues = new[] { ScheduledActionStatus.Scheduled, ScheduledActionStatus.Succeeded };
        queryBuilder.StatusIn(statues);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?sys.status[in]={System.Net.WebUtility.UrlEncode(string.Join(",", statues.Select(s => s.ToString().ToLower())))}", queryString);
    }

    [Fact]
    public void ScheduledForExactMatchIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        DateTime scheduledFor = DateTime.UtcNow;
        queryBuilder.ScheduledForExactMatch(scheduledFor);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?scheduledFor.datetime={System.Net.WebUtility.UrlEncode(scheduledFor.ToString("o"))}", queryString);
    }

    [Fact]
    public void ScheduledForLessThanIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        DateTime scheduledFor = DateTime.UtcNow;
        queryBuilder.ScheduledForLessThan(scheduledFor);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?scheduledFor.datetime[lt]={System.Net.WebUtility.UrlEncode(scheduledFor.ToString("o"))}", queryString);
    }

    [Fact]
    public void ScheduledForLessThanOrEqualIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        DateTime scheduledFor = DateTime.UtcNow;
        queryBuilder.ScheduledForLessThanOrEqual(scheduledFor);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?scheduledFor.datetime[lte]={System.Net.WebUtility.UrlEncode(scheduledFor.ToString("o"))}", queryString);
    }

    [Fact]
    public void ScheduledForGreaterThanIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        DateTime scheduledFor = DateTime.UtcNow;
        queryBuilder.ScheduledForGreaterThan(scheduledFor);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?scheduledFor.datetime[gt]={System.Net.WebUtility.UrlEncode(scheduledFor.ToString("o"))}", queryString);
    }

    [Fact]
    public void ScheduledForGreaterThanOrEqualIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        DateTime scheduledFor = DateTime.UtcNow;
        queryBuilder.ScheduledForGreaterThanOrEqual(scheduledFor);

        var queryString = queryBuilder.Build();

        Assert.Equal($"?scheduledFor.datetime[gte]={System.Net.WebUtility.UrlEncode(scheduledFor.ToString("o"))}", queryString);
    }

    [Fact]
    public void OrderByAscendinglIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();

        queryBuilder.OrderByScheduledFor();

        var queryString = queryBuilder.Build();

        Assert.Equal(string.Empty, queryString);
    }

    [Fact]
    public void OrderByDescendinglIsCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();

        queryBuilder.OrderByScheduledFor(false);

        var queryString = queryBuilder.Build();

        Assert.Equal("?order=-scheduledFor.datetime", queryString);
    }

    [Fact]
    public void MultipleSetsGivesCorrectQueryString()
    {
        var queryBuilder = new ScheduledActionQueryBuilder();
        var entityId = "123";
        var environmentId = "master";

        queryBuilder
            .EnvironmentIs(environmentId)
            .EntityIdIs(entityId);
        
        var queryString = queryBuilder.Build();

        Assert.Equal($"?environment.sys.id={environmentId}&entity.sys.id={entityId}", queryString);
    }
}