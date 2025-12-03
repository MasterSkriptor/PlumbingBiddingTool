using PlumbingBiddingTool.Application.FixtureItems;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using Moq;

namespace PlumbingBiddingTool.Application.Tests;

public class FixtureItemServiceTests
{
    private readonly Mock<IFixtureItemRepository> _mockFixtureRepository;
    private readonly Mock<IBidItemRepository> _mockBidItemRepository;
    private readonly FixtureItemService _service;

    public FixtureItemServiceTests()
    {
        _mockFixtureRepository = new Mock<IFixtureItemRepository>();
        _mockBidItemRepository = new Mock<IBidItemRepository>();
        _service = new FixtureItemService(_mockFixtureRepository.Object, _mockBidItemRepository.Object);
    }

    [Fact]
    public async Task GetAllFixtureItemsAsync_ShouldReturnAllItems()
    {
        // Arrange
        var expectedItems = new List<FixtureItem>
        {
            new FixtureItem { Id = 1, Name = "Fixture 1" },
            new FixtureItem { Id = 2, Name = "Fixture 2" }
        };
        _mockFixtureRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await _service.GetAllFixtureItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockFixtureRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetFixtureItemByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        var expectedItem = new FixtureItem { Id = 1, Name = "Test Fixture" };
        _mockFixtureRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetFixtureItemByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Fixture", result.Name);
        _mockFixtureRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetFixtureItemByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockFixtureRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FixtureItem?)null);

        // Act
        var result = await _service.GetFixtureItemByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockFixtureRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateFixtureItemAsync_ShouldCreateWithValidBidItems()
    {
        // Arrange
        var name = "New Fixture";
        var bidItemIds = new List<int> { 1, 2 };
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 20.00m };

        _mockBidItemRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bidItem1);
        _mockBidItemRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(bidItem2);
        _mockFixtureRepository.Setup(r => r.AddAsync(It.IsAny<FixtureItem>()))
            .ReturnsAsync((FixtureItem item) => item);

        // Act
        var result = await _service.CreateFixtureItemAsync(name, bidItemIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(2, result.BidItems.Count);
        _mockBidItemRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockBidItemRepository.Verify(r => r.GetByIdAsync(2), Times.Once);
        _mockFixtureRepository.Verify(r => r.AddAsync(It.Is<FixtureItem>(
            f => f.Name == name && f.BidItems.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task CreateFixtureItemAsync_ShouldSkipInvalidBidItems()
    {
        // Arrange
        var name = "New Fixture";
        var bidItemIds = new List<int> { 1, 999 }; // 999 doesn't exist
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m };

        _mockBidItemRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bidItem1);
        _mockBidItemRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BidItem?)null);
        _mockFixtureRepository.Setup(r => r.AddAsync(It.IsAny<FixtureItem>()))
            .ReturnsAsync((FixtureItem item) => item);

        // Act
        var result = await _service.CreateFixtureItemAsync(name, bidItemIds);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.BidItems); // Only valid bid item added
        _mockBidItemRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockBidItemRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateFixtureItemAsync_ShouldCreateWithNoBidItems()
    {
        // Arrange
        var name = "Empty Fixture";
        var bidItemIds = new List<int>();

        _mockFixtureRepository.Setup(r => r.AddAsync(It.IsAny<FixtureItem>()))
            .ReturnsAsync((FixtureItem item) => item);

        // Act
        var result = await _service.CreateFixtureItemAsync(name, bidItemIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Empty(result.BidItems);
        _mockFixtureRepository.Verify(r => r.AddAsync(It.Is<FixtureItem>(
            f => f.Name == name && f.BidItems.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task UpdateFixtureItemAsync_ShouldUpdateExistingItem()
    {
        // Arrange
        var existingItem = new FixtureItem 
        { 
            Id = 1, 
            Name = "Old Name",
            BidItems = new List<BidItem> 
            { 
                new BidItem { Id = 1, Name = "Old Bid", Price = 10.00m } 
            }
        };
        var newBidItem = new BidItem { Id = 2, Name = "New Bid", Price = 20.00m };

        _mockFixtureRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
        _mockBidItemRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newBidItem);
        _mockFixtureRepository.Setup(r => r.UpdateAsync(It.IsAny<FixtureItem>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateFixtureItemAsync(1, "New Name", new List<int> { 2 });

        // Assert
        Assert.Equal("New Name", existingItem.Name);
        Assert.Single(existingItem.BidItems);
        Assert.Equal(2, existingItem.BidItems.First().Id);
        _mockFixtureRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockFixtureRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task UpdateFixtureItemAsync_ShouldNotUpdate_WhenItemNotFound()
    {
        // Arrange
        _mockFixtureRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FixtureItem?)null);

        // Act
        await _service.UpdateFixtureItemAsync(999, "New Name", new List<int> { 1 });

        // Assert
        _mockFixtureRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockFixtureRepository.Verify(r => r.UpdateAsync(It.IsAny<FixtureItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFixtureItemAsync_ShouldClearBidItems_WhenEmptyList()
    {
        // Arrange
        var existingItem = new FixtureItem 
        { 
            Id = 1, 
            Name = "Fixture",
            BidItems = new List<BidItem> 
            { 
                new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m } 
            }
        };

        _mockFixtureRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
        _mockFixtureRepository.Setup(r => r.UpdateAsync(It.IsAny<FixtureItem>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateFixtureItemAsync(1, "Updated Fixture", new List<int>());

        // Assert
        Assert.Empty(existingItem.BidItems);
        _mockFixtureRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task DeleteFixtureItemAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        _mockFixtureRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteFixtureItemAsync(1);

        // Assert
        _mockFixtureRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetAvailableBidItemsAsync_ShouldReturnAllBidItems()
    {
        // Arrange
        var expectedBidItems = new List<BidItem>
        {
            new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m },
            new BidItem { Id = 2, Name = "Bid 2", Price = 20.00m }
        };
        _mockBidItemRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedBidItems);

        // Act
        var result = await _service.GetAvailableBidItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBidItemRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
