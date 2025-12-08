using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Infrastructure.Data;
using PlumbingBiddingTool.Infrastructure.Repositories;

namespace PlumbingBiddingTool.Infrastructure.Tests;

public class BidItemRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BidItemRepository _repository;

    public BidItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BidItemRepository(_context);
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
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Arrange
        _context.BidItems.AddRange(
            new BidItem { Id = 1, Name = "Item 1", Price = 10.00m, Phase = Phase.Underground },
            new BidItem { Id = 2, Name = "Item 2", Price = 20.00m, Phase = Phase.StackOut },
            new BidItem { Id = 3, Name = "Item 3", Price = 30.00m, Phase = Phase.Trim }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        var item = new BidItem { Id = 1, Name = "Test Item", Price = 15.50m, Phase = Phase.Trim };
        _context.BidItems.Add(item);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Item", result.Name);
        Assert.Equal(15.50m, result.Price);
        Assert.Equal(Phase.Trim, result.Phase);
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
    public async Task AddAsync_ShouldAddNewItem()
    {
        // Arrange
        var newItem = new BidItem { Name = "New Item", Price = 25.99m, Phase = Phase.StackOut };

        // Act
        var result = await _repository.AddAsync(newItem);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Item", result.Name);
        Assert.Equal(25.99m, result.Price);
        Assert.Equal(Phase.StackOut, result.Phase);
        
        // Verify it's in the database
        var dbItem = await _context.BidItems.FindAsync(result.Id);
        Assert.NotNull(dbItem);
        Assert.Equal("New Item", dbItem.Name);
        Assert.Equal(Phase.StackOut, dbItem.Phase);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var item = new BidItem { Name = "Persistent Item", Price = 100.00m, Phase = Phase.Underground };

        // Act
        await _repository.AddAsync(item);

        // Assert
        var count = await _context.BidItems.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingItem()
    {
        // Arrange
        var item = new BidItem { Id = 1, Name = "Original", Price = 10.00m, Phase = Phase.Underground };
        _context.BidItems.Add(item);
        await _context.SaveChangesAsync();

        // Act
        item.Name = "Updated";
        item.Price = 20.00m;
        item.Phase = Phase.Trim;
        await _repository.UpdateAsync(item);

        // Assert
        var updatedItem = await _context.BidItems.FindAsync(1);
        Assert.NotNull(updatedItem);
        Assert.Equal("Updated", updatedItem.Name);
        Assert.Equal(20.00m, updatedItem.Price);
        Assert.Equal(Phase.Trim, updatedItem.Phase);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var item = new BidItem { Id = 1, Name = "Before", Price = 5.00m, Phase = Phase.StackOut };
        _context.BidItems.Add(item);
        await _context.SaveChangesAsync();
        
        // Detach to simulate fresh context
        _context.Entry(item).State = EntityState.Detached;

        // Act
        var modifiedItem = new BidItem { Id = 1, Name = "After", Price = 15.00m, Phase = Phase.Trim };
        await _repository.UpdateAsync(modifiedItem);

        // Assert
        var result = await _context.BidItems.FindAsync(1);
        Assert.Equal("After", result!.Name);
        Assert.Equal(15.00m, result.Price);
        Assert.Equal(Phase.Trim, result.Phase);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItem_WhenExists()
    {
        // Arrange
        var item = new BidItem { Id = 1, Name = "To Delete", Price = 5.00m, Phase = Phase.Underground };
        _context.BidItems.Add(item);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deletedItem = await _context.BidItems.FindAsync(1);
        Assert.Null(deletedItem);
        Assert.Equal(0, await _context.BidItems.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenItemNotExists()
    {
        // Arrange
        _context.BidItems.Add(new BidItem { Id = 1, Name = "Item", Price = 10.00m, Phase = Phase.Trim });
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(999);

        // Assert
        Assert.Equal(1, await _context.BidItems.CountAsync());
    }

    [Theory]
    [InlineData("Copper Pipe", 25.50)]
    [InlineData("Ball Valve", 15.00)]
    [InlineData("PVC Fitting", 3.99)]
    public async Task AddAsync_ShouldHandleVariousInputs(string name, decimal price)
    {
        // Arrange
        var item = new BidItem { Name = name, Price = price, Phase = Phase.StackOut };

        // Act
        var result = await _repository.AddAsync(item);

        // Assert
        Assert.Equal(name, result.Name);
        Assert.Equal(price, result.Price);
        Assert.Equal(Phase.StackOut, result.Phase);
    }

    [Fact]
    public async Task Repository_ShouldHandleMultipleOperations()
    {
        // Arrange & Act
        var item1 = await _repository.AddAsync(new BidItem { Name = "Item 1", Price = 10m, Phase = Phase.Underground });
        var item2 = await _repository.AddAsync(new BidItem { Name = "Item 2", Price = 20m, Phase = Phase.StackOut });
        
        item1.Name = "Modified Item 1";
        await _repository.UpdateAsync(item1);
        
        await _repository.DeleteAsync(item2.Id);

        // Assert
        var allItems = await _repository.GetAllAsync();
        Assert.Single(allItems);
        Assert.Equal("Modified Item 1", allItems.First().Name);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
