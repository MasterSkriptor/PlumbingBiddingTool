using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Tests;

public class FixtureItemTests
{
    [Fact]
    public void FixtureItem_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var fixtureItem = new FixtureItem();

        // Assert
        Assert.Equal(0, fixtureItem.Id);
        Assert.Equal(string.Empty, fixtureItem.Name);
        Assert.Empty(fixtureItem.BidItems);
        Assert.Equal(0, fixtureItem.Price);
    }

    [Fact]
    public void FixtureItem_ShouldAllowSettingProperties()
    {
        // Arrange
        var fixtureItem = new FixtureItem();
        var bidItem = new BidItem { Id = 1, Name = "Test Bid", Price = 50.00m, Phase = Phase.Underground };

        // Act
        fixtureItem.Id = 1;
        fixtureItem.Name = "Test Fixture";
        fixtureItem.BidItems.Add(bidItem);

        // Assert
        Assert.Equal(1, fixtureItem.Id);
        Assert.Equal("Test Fixture", fixtureItem.Name);
        Assert.Single(fixtureItem.BidItems);
        Assert.Equal(50.00m, fixtureItem.Price);
    }

    [Fact]
    public void FixtureItem_Price_ShouldCalculateSumOfBidItems()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            Name = "Bathroom Fixture",
            BidItems = new List<BidItem>
            {
                new BidItem { Id = 1, Name = "Sink", Price = 150.00m, Phase = Phase.Underground },
                new BidItem { Id = 2, Name = "Faucet", Price = 75.50m, Phase = Phase.StackOut },
                new BidItem { Id = 3, Name = "Drain", Price = 25.25m, Phase = Phase.Trim }
            }
        };

        // Act
        var totalPrice = fixtureItem.Price;

        // Assert
        Assert.Equal(250.75m, totalPrice);
    }

    [Fact]
    public void FixtureItem_Price_ShouldBeZero_WhenNoBidItems()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            Name = "Empty Fixture"
        };

        // Act
        var price = fixtureItem.Price;

        // Assert
        Assert.Equal(0, price);
    }

    [Fact]
    public void FixtureItem_Price_ShouldUpdateDynamically_WhenBidItemsChange()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            Name = "Dynamic Fixture",
            BidItems = new List<BidItem>
            {
                new BidItem { Id = 1, Name = "Item 1", Price = 100.00m, Phase = Phase.Underground }
            }
        };

        var initialPrice = fixtureItem.Price;

        // Act
        fixtureItem.BidItems.Add(new BidItem { Id = 2, Name = "Item 2", Price = 50.00m, Phase = Phase.StackOut });
        var updatedPrice = fixtureItem.Price;

        // Assert
        Assert.Equal(100.00m, initialPrice);
        Assert.Equal(150.00m, updatedPrice);
    }

    [Theory]
    [InlineData("Kitchen Fixture")]
    [InlineData("Bathroom Fixture")]
    [InlineData("Utility Room Fixture")]
    public void FixtureItem_ShouldStoreVariousNames(string name)
    {
        // Arrange & Act
        var fixtureItem = new FixtureItem
        {
            Name = name
        };

        // Assert
        Assert.Equal(name, fixtureItem.Name);
    }

    [Fact]
    public void FixtureItem_ShouldHandleMultipleBidItems()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            Name = "Complex Fixture",
            BidItems = new List<BidItem>
            {
                new BidItem { Id = 1, Name = "Item 1", Price = 10.00m, Phase = Phase.Underground },
                new BidItem { Id = 2, Name = "Item 2", Price = 20.00m, Phase = Phase.StackOut },
                new BidItem { Id = 3, Name = "Item 3", Price = 30.00m, Phase = Phase.Trim },
                new BidItem { Id = 4, Name = "Item 4", Price = 40.00m, Phase = Phase.Underground },
                new BidItem { Id = 5, Name = "Item 5", Price = 50.00m, Phase = Phase.StackOut }
            }
        };

        // Act & Assert
        Assert.Equal(5, fixtureItem.BidItems.Count);
        Assert.Equal(150.00m, fixtureItem.Price);
    }

    [Fact]
    public void FixtureItem_Price_ShouldHandleDecimalPrecision()
    {
        // Arrange
        var fixtureItem = new FixtureItem
        {
            BidItems = new List<BidItem>
            {
                new BidItem { Price = 10.99m, Phase = Phase.Underground },
                new BidItem { Price = 20.01m, Phase = Phase.StackOut },
                new BidItem { Price = 5.555m, Phase = Phase.Trim }
            }
        };

        // Act
        var totalPrice = fixtureItem.Price;

        // Assert
        Assert.Equal(36.555m, totalPrice);
    }
}
