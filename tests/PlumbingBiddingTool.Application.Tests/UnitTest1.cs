using PlumbingBiddingTool.Application.BidItems;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using Moq;

namespace PlumbingBiddingTool.Application.Tests;

public class BidItemServiceTests
{
    private readonly Mock<IBidItemRepository> _mockRepository;
    private readonly BidItemService _service;

    public BidItemServiceTests()
    {
        _mockRepository = new Mock<IBidItemRepository>();
        _service = new BidItemService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllBidItemsAsync_ShouldReturnAllItems()
    {
        // Arrange
        var expectedItems = new List<BidItem>
        {
            new BidItem { Id = 1, Name = "Item 1", Price = 10.00m, Phase = Phase.Underground },
            new BidItem { Id = 2, Name = "Item 2", Price = 20.00m, Phase = Phase.StackOut }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await _service.GetAllBidItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBidItemByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        var expectedItem = new BidItem { Id = 1, Name = "Test Item", Price = 15.50m, Phase = Phase.Trim };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetBidItemByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Item", result.Name);
        Assert.Equal(15.50m, result.Price);
        Assert.Equal(Phase.Trim, result.Phase);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetBidItemByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BidItem?)null);

        // Act
        var result = await _service.GetBidItemByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateBidItemAsync_ShouldCreateAndReturnNewItem()
    {
        // Arrange
        var name = "New Item";
        var price = 25.99m;
        var phase = Phase.StackOut;
        var createdItem = new BidItem { Id = 1, Name = name, Price = price, Phase = phase };
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<BidItem>()))
            .ReturnsAsync((BidItem item) => 
            {
                item.Id = 1;
                return item;
            });

        // Act
        var result = await _service.CreateBidItemAsync(name, price, phase);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(price, result.Price);
        Assert.Equal(phase, result.Phase);
        _mockRepository.Verify(r => r.AddAsync(It.Is<BidItem>(
            b => b.Name == name && b.Price == price && b.Phase == phase)), Times.Once);
    }

    [Fact]
    public async Task UpdateBidItemAsync_ShouldUpdateExistingItem()
    {
        // Arrange
        var existingItem = new BidItem { Id = 1, Name = "Old Name", Price = 10.00m, Phase = Phase.Underground };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<BidItem>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateBidItemAsync(1, "New Name", 20.00m, Phase.Trim);

        // Assert
        Assert.Equal("New Name", existingItem.Name);
        Assert.Equal(20.00m, existingItem.Price);
        Assert.Equal(Phase.Trim, existingItem.Phase);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task UpdateBidItemAsync_ShouldNotUpdate_WhenItemNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BidItem?)null);

        // Act
        await _service.UpdateBidItemAsync(999, "New Name", 20.00m, Phase.StackOut);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<BidItem>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBidItemAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBidItemAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Theory]
    [InlineData("Copper Pipe", 25.50)]
    [InlineData("Ball Valve", 15.00)]
    [InlineData("PVC Fitting", 3.99)]
    public async Task CreateBidItemAsync_ShouldHandleVariousInputs(string name, decimal price)
    {
        // Arrange
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<BidItem>()))
            .ReturnsAsync((BidItem item) => item);

        // Act
        var result = await _service.CreateBidItemAsync(name, price, Phase.Underground);

        // Assert
        Assert.Equal(name, result.Name);
        Assert.Equal(price, result.Price);
        Assert.Equal(Phase.Underground, result.Phase);
    }
}
