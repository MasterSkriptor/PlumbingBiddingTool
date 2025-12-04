using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Tests;

public class ContractorTests
{
    [Fact]
    public void Contractor_CanBeCreated_WithValidProperties()
    {
        // Arrange & Act
        var contractor = new Contractor
        {
            Id = 1,
            Name = "ABC Plumbing"
        };

        // Assert
        Assert.Equal(1, contractor.Id);
        Assert.Equal("ABC Plumbing", contractor.Name);
        Assert.NotNull(contractor.Jobs);
    }

    [Fact]
    public void Contractor_Jobs_InitializesAsEmptyCollection()
    {
        // Arrange & Act
        var contractor = new Contractor
        {
            Name = "Test Contractor"
        };

        // Assert
        Assert.NotNull(contractor.Jobs);
        Assert.Empty(contractor.Jobs);
    }

    [Fact]
    public void Contractor_CanAddJobs()
    {
        // Arrange
        var contractor = new Contractor { Name = "Test Contractor" };
        var job = new Job
        {
            JobName = "Office Renovation",
            Status = JobStatus.Open
        };

        // Act
        contractor.Jobs.Add(job);

        // Assert
        Assert.Single(contractor.Jobs);
        Assert.Contains(job, contractor.Jobs);
    }
}
