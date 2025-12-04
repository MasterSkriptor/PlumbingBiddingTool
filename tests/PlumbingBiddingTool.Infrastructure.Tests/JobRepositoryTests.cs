using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Infrastructure.Data;
using PlumbingBiddingTool.Infrastructure.Repositories;

namespace PlumbingBiddingTool.Infrastructure.Tests;

public class JobRepositoryTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllJobs_WithRelatedData()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "ABC Plumbing" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        var job1 = new Job { JobName = "Office Renovation", ContractorId = contractor.Id, Status = JobStatus.Open };
        var job2 = new Job { JobName = "Home Plumbing", ContractorId = contractor.Id, Status = JobStatus.Accepted };

        context.Jobs.AddRange(job1, job2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, job => Assert.NotNull(job.Contractor));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsJob_WithAllRelatedData()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        var fixtureItem = new FixtureItem { Name = "Test Fixture" };
        fixtureItem.BidItems.Add(new BidItem { Name = "Bid Item 1", Price = 50.00m });
        
        context.Contractors.Add(contractor);
        context.FixtureItems.Add(fixtureItem);
        await context.SaveChangesAsync();

        var job = new Job { JobName = "Test Job", ContractorId = contractor.Id, Status = JobStatus.Open };
        job.JobFixtureItems.Add(new JobFixtureItem
        {
            FixtureItemId = fixtureItem.Id,
            Price = 50.00m,
            Quantity = 2
        });
        
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(job.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Job", result.JobName);
        Assert.NotNull(result.Contractor);
        Assert.Single(result.JobFixtureItems);
        Assert.NotNull(result.JobFixtureItems.First().FixtureItem);
        Assert.NotEmpty(result.JobFixtureItems.First().FixtureItem.BidItems);
    }

    [Fact]
    public async Task GetByContractorIdAsync_ReturnsFilteredJobs()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor1 = new Contractor { Name = "Contractor 1" };
        var contractor2 = new Contractor { Name = "Contractor 2" };
        context.Contractors.AddRange(contractor1, contractor2);
        await context.SaveChangesAsync();

        context.Jobs.AddRange(
            new Job { JobName = "Job 1", ContractorId = contractor1.Id, Status = JobStatus.Open },
            new Job { JobName = "Job 2", ContractorId = contractor1.Id, Status = JobStatus.Accepted },
            new Job { JobName = "Job 3", ContractorId = contractor2.Id, Status = JobStatus.Open }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByContractorIdAsync(contractor1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, job => Assert.Equal(contractor1.Id, job.ContractorId));
    }

    [Fact]
    public async Task AddAsync_AddsJob()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        var job = new Job
        {
            JobName = "New Job",
            ContractorId = contractor.Id,
            Status = JobStatus.Open
        };

        // Act
        var result = await repository.AddAsync(job);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Job", result.JobName);
        Assert.Single(context.Jobs);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesJob()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        var job = new Job
        {
            JobName = "Original Name",
            ContractorId = contractor.Id,
            Status = JobStatus.Open
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        // Act
        job.JobName = "Updated Name";
        job.Status = JobStatus.Accepted;
        await repository.UpdateAsync(job);

        // Assert
        var updated = await context.Jobs.FindAsync(job.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.JobName);
        Assert.Equal(JobStatus.Accepted, updated.Status);
    }

    [Fact]
    public async Task DeleteAsync_DeletesJob()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        var job = new Job
        {
            JobName = "To Delete",
            ContractorId = contractor.Id,
            Status = JobStatus.Open
        };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(job.Id);

        // Assert
        var deleted = await context.Jobs.FindAsync(job.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Job_CalculatesTotalCost_FromJobFixtureItems()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repository = new JobRepository(context);

        var contractor = new Contractor { Name = "Test Contractor" };
        var fixture1 = new FixtureItem { Name = "Fixture 1" };
        var fixture2 = new FixtureItem { Name = "Fixture 2" };
        
        context.Contractors.Add(contractor);
        context.FixtureItems.AddRange(fixture1, fixture2);
        await context.SaveChangesAsync();

        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = contractor.Id,
            Status = JobStatus.Open
        };
        job.JobFixtureItems.Add(new JobFixtureItem
        {
            FixtureItemId = fixture1.Id,
            Price = 50.00m,
            Quantity = 3
        });
        job.JobFixtureItems.Add(new JobFixtureItem
        {
            FixtureItemId = fixture2.Id,
            Price = 75.00m,
            Quantity = 2
        });

        context.Jobs.Add(job);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(job.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(300.00m, result.TotalCost); // (50*3) + (75*2) = 150 + 150 = 300
    }
}
