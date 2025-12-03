using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Infrastructure.Data;
using PlumbingBiddingTool.Infrastructure.Repositories;

namespace PlumbingBiddingTool.Infrastructure.Tests;

public class FixtureItemRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly FixtureItemRepository _repository;
    private readonly BidItemRepository _bidItemRepository;

    public FixtureItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new FixtureItemRepository(_context);
        _bidItemRepository = new BidItemRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoItems()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFixtureItems()
    {
        // Arrange
        _context.FixtureItems.AddRange(
            new FixtureItem { Id = 1, Name = "Fixture 1" },
            new FixtureItem { Id = 2, Name = "Fixture 2" },
            new FixtureItem { Id = 3, Name = "Fixture 3" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeBidItems()
    {
        // Arrange
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 20.00m };
        _context.BidItems.AddRange(bidItem1, bidItem2);

        var fixture = new FixtureItem 
        { 
            Id = 1, 
            Name = "Test Fixture",
            BidItems = new List<BidItem> { bidItem1, bidItem2 }
        };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var fixtureItem = result.First();

        // Assert
        Assert.Equal(2, fixtureItem.BidItems.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        var fixture = new FixtureItem { Id = 1, Name = "Test Fixture" };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Fixture", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeBidItems()
    {
        // Arrange
        var bidItem = new BidItem { Id = 1, Name = "Bid Item", Price = 50.00m };
        _context.BidItems.Add(bidItem);

        var fixture = new FixtureItem 
        { 
            Id = 1, 
            Name = "Test Fixture",
            BidItems = new List<BidItem> { bidItem }
        };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.BidItems);
        Assert.Equal("Bid Item", result.BidItems.First().Name);
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewFixtureItem()
    {
        // Arrange
        var newFixture = new FixtureItem { Name = "New Fixture" };

        // Act
        var result = await _repository.AddAsync(newFixture);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Fixture", result.Name);
        
        var dbItem = await _context.FixtureItems.FindAsync(result.Id);
        Assert.NotNull(dbItem);
    }

    [Fact]
    public async Task AddAsync_ShouldAddFixtureWithBidItems()
    {
        // Arrange
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 20.00m };
        _context.BidItems.AddRange(bidItem1, bidItem2);
        await _context.SaveChangesAsync();

        var newFixture = new FixtureItem 
        { 
            Name = "Fixture with Items",
            BidItems = new List<BidItem> { bidItem1, bidItem2 }
        };

        // Act
        var result = await _repository.AddAsync(newFixture);

        // Assert
        Assert.Equal(2, result.BidItems.Count);
        
        var dbItem = await _context.FixtureItems
            .Include(f => f.BidItems)
            .FirstOrDefaultAsync(f => f.Id == result.Id);
        Assert.NotNull(dbItem);
        Assert.Equal(2, dbItem.BidItems.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingFixtureItem()
    {
        // Arrange
        var fixture = new FixtureItem { Id = 1, Name = "Original" };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        fixture.Name = "Updated";
        await _repository.UpdateAsync(fixture);

        // Assert
        var updatedItem = await _context.FixtureItems.FindAsync(1);
        Assert.NotNull(updatedItem);
        Assert.Equal("Updated", updatedItem.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBidItems()
    {
        // Arrange
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 10.00m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 20.00m };
        _context.BidItems.AddRange(bidItem1, bidItem2);

        var fixture = new FixtureItem 
        { 
            Id = 1, 
            Name = "Fixture",
            BidItems = new List<BidItem> { bidItem1 }
        };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Detach to simulate fresh context
        _context.Entry(fixture).State = EntityState.Detached;

        // Act
        var fixtureToUpdate = await _repository.GetByIdAsync(1);
        fixtureToUpdate!.BidItems.Clear();
        fixtureToUpdate.BidItems.Add(bidItem2);
        await _repository.UpdateAsync(fixtureToUpdate);

        // Assert
        var result = await _repository.GetByIdAsync(1);
        Assert.Single(result!.BidItems);
        Assert.Equal(2, result.BidItems.First().Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFixtureItem_WhenExists()
    {
        // Arrange
        var fixture = new FixtureItem { Id = 1, Name = "To Delete" };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedItem = await _context.FixtureItems.FindAsync(1);
        Assert.Null(deletedItem);
        Assert.Equal(0, await _context.FixtureItems.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenItemNotExists()
    {
        // Arrange
        _context.FixtureItems.Add(new FixtureItem { Id = 1, Name = "Item" });
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(999);

        // Assert
        Assert.Equal(1, await _context.FixtureItems.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotDeleteRelatedBidItems()
    {
        // Arrange
        var bidItem = new BidItem { Id = 1, Name = "Bid Item", Price = 10.00m };
        _context.BidItems.Add(bidItem);

        var fixture = new FixtureItem 
        { 
            Id = 1, 
            Name = "Fixture",
            BidItems = new List<BidItem> { bidItem }
        };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedFixture = await _context.FixtureItems.FindAsync(1);
        var bidItemStillExists = await _context.BidItems.FindAsync(1);
        
        Assert.Null(deletedFixture);
        Assert.NotNull(bidItemStillExists); // BidItem should not be deleted
    }

    [Fact]
    public async Task Repository_ShouldHandleComplexScenario()
    {
        // Arrange
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 100.00m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 200.00m };
        var bidItem3 = new BidItem { Id = 3, Name = "Bid 3", Price = 300.00m };
        _context.BidItems.AddRange(bidItem1, bidItem2, bidItem3);
        await _context.SaveChangesAsync();

        // Act - Create two fixtures
        var fixture1 = await _repository.AddAsync(new FixtureItem 
        { 
            Name = "Fixture 1",
            BidItems = new List<BidItem> { bidItem1, bidItem2 }
        });
        
        var fixture2 = await _repository.AddAsync(new FixtureItem 
        { 
            Name = "Fixture 2",
            BidItems = new List<BidItem> { bidItem2, bidItem3 }
        });

        // Update fixture1
        var fixtureToUpdate = await _repository.GetByIdAsync(fixture1.Id);
        fixtureToUpdate!.Name = "Updated Fixture 1";
        fixtureToUpdate.BidItems.Clear();
        fixtureToUpdate.BidItems.Add(bidItem3);
        await _repository.UpdateAsync(fixtureToUpdate);

        // Delete fixture2
        await _repository.DeleteAsync(fixture2.Id);

        // Assert
        var allFixtures = await _repository.GetAllAsync();
        Assert.Single(allFixtures);
        
        var remaining = allFixtures.First();
        Assert.Equal("Updated Fixture 1", remaining.Name);
        Assert.Single(remaining.BidItems);
        Assert.Equal(3, remaining.BidItems.First().Id);
        
        // All bid items should still exist
        Assert.Equal(3, await _context.BidItems.CountAsync());
    }

    [Fact]
    public async Task Repository_ShouldCalculatePriceCorrectly()
    {
        // Arrange
        var bidItem1 = new BidItem { Id = 1, Name = "Bid 1", Price = 25.50m };
        var bidItem2 = new BidItem { Id = 2, Name = "Bid 2", Price = 74.50m };
        _context.BidItems.AddRange(bidItem1, bidItem2);

        var fixture = new FixtureItem 
        { 
            Name = "Price Test Fixture",
            BidItems = new List<BidItem> { bidItem1, bidItem2 }
        };
        _context.FixtureItems.Add(fixture);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(fixture.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100.00m, result.Price);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
