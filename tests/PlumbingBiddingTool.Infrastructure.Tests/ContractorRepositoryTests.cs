using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Infrastructure.Data;
using PlumbingBiddingTool.Infrastructure.Repositories;

namespace PlumbingBiddingTool.Infrastructure.Tests;

public class ContractorRepositoryTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllContractors_WithJobs()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        var contractor1 = new Contractor { Name = "ABC Plumbing" };
        contractor1.Jobs.Add(new Job { JobName = "Job 1", Status = JobStatus.Open });
        
        var contractor2 = new Contractor { Name = "XYZ Contractors" };

        context.Contractors.AddRange(contractor1, contractor2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Single(result.First(c => c.Name == "ABC Plumbing").Jobs);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsContractor_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(contractor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Contractor", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsContractor()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);
        var contractor = new Contractor { Name = "New Contractor" };

        // Act
        var result = await repository.AddAsync(contractor);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Contractor", result.Name);
        Assert.Single(context.Contractors);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesContractor()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        var contractor = new Contractor { Name = "Original Name" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        // Act
        contractor.Name = "Updated Name";
        await repository.UpdateAsync(contractor);

        // Assert
        var updated = await context.Contractors.FindAsync(contractor.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesContractor()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        var contractor = new Contractor { Name = "To Delete" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(contractor.Id);

        // Assert
        var deleted = await context.Contractors.FindAsync(contractor.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_WhenContractorNotExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new ContractorRepository(context);

        // Act & Assert
        await repository.DeleteAsync(999); // Should not throw
    }
}
