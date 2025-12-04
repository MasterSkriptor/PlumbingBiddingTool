using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Tests;

public class JobTests
{
    [Fact]
    public void Job_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var job = new Job
        {
            Id = 1,
            JobName = "Office Renovation",
            ContractorId = 1,
            Status = JobStatus.Open
        };

        // Assert
        Assert.Equal(1, job.Id);
        Assert.Equal("Office Renovation", job.JobName);
        Assert.Equal(1, job.ContractorId);
        Assert.Equal(JobStatus.Open, job.Status);
        Assert.NotNull(job.JobFixtureItems);
    }

    [Fact]
    public void Job_DefaultStatus_IsOpen()
    {
        // Arrange & Act
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };

        // Assert
        Assert.Equal(JobStatus.Open, job.Status);
    }

    [Fact]
    public void Job_TotalCost_CalculatesCorrectly_WithNoItems()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };

        // Act
        var totalCost = job.TotalCost;

        // Assert
        Assert.Equal(0, totalCost);
    }

    [Fact]
    public void Job_TotalCost_CalculatesCorrectly_WithSingleItem()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };
        var jobFixtureItem = new JobFixtureItem
        {
            Price = 100.50m,
            Quantity = 2
        };
        job.JobFixtureItems.Add(jobFixtureItem);

        // Act
        var totalCost = job.TotalCost;

        // Assert
        Assert.Equal(201.00m, totalCost);
    }

    [Fact]
    public void Job_TotalCost_CalculatesCorrectly_WithMultipleItems()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };
        job.JobFixtureItems.Add(new JobFixtureItem { Price = 50.00m, Quantity = 3 });
        job.JobFixtureItems.Add(new JobFixtureItem { Price = 75.25m, Quantity = 2 });
        job.JobFixtureItems.Add(new JobFixtureItem { Price = 100.00m, Quantity = 1 });

        // Act
        var totalCost = job.TotalCost;

        // Assert
        Assert.Equal(400.50m, totalCost); // (50*3) + (75.25*2) + (100*1)
    }

    [Fact]
    public void Job_TotalCost_HandlesZeroQuantity()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };
        job.JobFixtureItems.Add(new JobFixtureItem { Price = 100.00m, Quantity = 0 });

        // Act
        var totalCost = job.TotalCost;

        // Assert
        Assert.Equal(0, totalCost);
    }

    [Fact]
    public void Job_TotalCost_UpdatesDynamically_WhenItemsChange()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1
        };
        var item = new JobFixtureItem { Price = 50.00m, Quantity = 2 };
        job.JobFixtureItems.Add(item);

        var initialCost = job.TotalCost;

        // Act
        item.Quantity = 5;
        var updatedCost = job.TotalCost;

        // Assert
        Assert.Equal(100.00m, initialCost);
        Assert.Equal(250.00m, updatedCost);
    }

    [Fact]
    public void Job_StatusCanBeChanged()
    {
        // Arrange
        var job = new Job
        {
            JobName = "Test Job",
            ContractorId = 1,
            Status = JobStatus.Open
        };

        // Act
        job.Status = JobStatus.Accepted;

        // Assert
        Assert.Equal(JobStatus.Accepted, job.Status);
    }
}
