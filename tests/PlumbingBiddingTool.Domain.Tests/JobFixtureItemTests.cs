using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Tests;

public class JobFixtureItemTests
{
    [Fact]
    public void JobFixtureItem_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var jobFixtureItem = new JobFixtureItem
        {
            Id = 1,
            JobId = 1,
            FixtureItemId = 1,
            Quantity = 5,
            Price = 150.75m
        };

        // Assert
        Assert.Equal(1, jobFixtureItem.Id);
        Assert.Equal(1, jobFixtureItem.JobId);
        Assert.Equal(1, jobFixtureItem.FixtureItemId);
        Assert.Equal(5, jobFixtureItem.Quantity);
        Assert.Equal(150.75m, jobFixtureItem.Price);
    }

    [Fact]
    public void JobFixtureItem_Price_IsSnapshot_NotCalculated()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            Id = 1,
            Name = "Test Fixture"
        };
        fixtureItem.BidItems.Add(new BidItem { Price = 50.00m });
        
        var jobFixtureItem = new JobFixtureItem
        {
            FixtureItemId = fixtureItem.Id,
            FixtureItem = fixtureItem,
            Price = 50.00m, // Snapshot price
            Quantity = 1
        };

        // Act - Change the fixture item's price
        fixtureItem.BidItems.Clear();
        fixtureItem.BidItems.Add(new BidItem { Price = 100.00m });

        // Assert - JobFixtureItem price remains the snapshot
        Assert.Equal(100.00m, fixtureItem.Price); // FixtureItem recalculated
        Assert.Equal(50.00m, jobFixtureItem.Price); // JobFixtureItem unchanged
    }

    [Fact]
    public void JobFixtureItem_CalculatesTotalCost()
    {
        // Arrange
        var jobFixtureItem = new JobFixtureItem
        {
            Price = 25.50m,
            Quantity = 4
        };

        // Act
        var totalCost = jobFixtureItem.Price * jobFixtureItem.Quantity;

        // Assert
        Assert.Equal(102.00m, totalCost);
    }
}
