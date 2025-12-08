using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Tests;

public class BidItemTests
{
    [Fact]
    public void BidItem_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var bidItem = new BidItem();

        // Assert
        Assert.Equal(0, bidItem.Id);
        Assert.Equal(string.Empty, bidItem.Name);
        Assert.Equal(0, bidItem.Price);
        Assert.Equal(Phase.Underground, bidItem.Phase);
    }

    [Fact]
    public void BidItem_ShouldAllowSettingProperties()
    {
        // Arrange
        var bidItem = new BidItem();

        // Act
        bidItem.Id = 1;
        bidItem.Name = "Test Item";
        bidItem.Price = 99.99m;
        bidItem.Phase = Phase.StackOut;

        // Assert
        Assert.Equal(1, bidItem.Id);
        Assert.Equal("Test Item", bidItem.Name);
        Assert.Equal(99.99m, bidItem.Price);
        Assert.Equal(Phase.StackOut, bidItem.Phase);
    }

    [Theory]
    [InlineData("Copper Pipe", 25.50)]
    [InlineData("Ball Valve", 15.00)]
    [InlineData("PVC Fitting", 3.99)]
    public void BidItem_ShouldStoreVariousNameAndPriceCombinations(string name, decimal price)
    {
        // Arrange & Act
        var bidItem = new BidItem
        {
            Name = name,
            Price = price
        };

        // Assert
        Assert.Equal(name, bidItem.Name);
        Assert.Equal(price, bidItem.Price);
    }

    [Fact]
    public void BidItem_ShouldHandleDecimalPrecision()
    {
        // Arrange & Act
        var bidItem = new BidItem
        {
            Price = 123.456789m
        };

        // Assert
        Assert.Equal(123.456789m, bidItem.Price);
    }

    [Theory]
    [InlineData(Phase.Underground)]
    [InlineData(Phase.StackOut)]
    [InlineData(Phase.Trim)]
    public void BidItem_ShouldStoreAllPhaseValues(Phase phase)
    {
        // Arrange & Act
        var bidItem = new BidItem
        {
            Name = "Test Item",
            Price = 50.00m,
            Phase = phase
        };

        // Assert
        Assert.Equal(phase, bidItem.Phase);
    }

    [Fact]
    public void BidItem_DefaultPhase_ShouldBeUnderground()
    {
        // Arrange & Act
        var bidItem = new BidItem
        {
            Name = "Test Item",
            Price = 50.00m
        };

        // Assert
        Assert.Equal(Phase.Underground, bidItem.Phase);
    }
}
