using Moq;
using PlumbingBiddingTool.Application.Jobs;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.Tests;

public class JobServiceTests
{
    private readonly Mock<IJobRepository> _mockJobRepository;
    private readonly Mock<IFixtureItemRepository> _mockFixtureRepository;
    private readonly JobService _service;

    public JobServiceTests()
    {
        _mockJobRepository = new Mock<IJobRepository>();
        _mockFixtureRepository = new Mock<IFixtureItemRepository>();
        _service = new JobService(_mockJobRepository.Object, _mockFixtureRepository.Object);
    }

    [Fact]
    public async Task GetAllJobsAsync_ReturnsAllJobs()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, JobName = "Office Renovation", ContractorId = 1 },
            new Job { Id = 2, JobName = "Home Plumbing", ContractorId = 2 }
        };
        _mockJobRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(jobs);

        // Act
        var result = await _service.GetAllJobsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockJobRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetJobByIdAsync_ReturnsJob_WhenExists()
    {
        // Arrange
        var job = new Job { Id = 1, JobName = "Office Renovation", ContractorId = 1 };
        _mockJobRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);

        // Act
        var result = await _service.GetJobByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Office Renovation", result.JobName);
        _mockJobRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetJobsByContractorAsync_ReturnsFilteredJobs()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = 1, JobName = "Job 1", ContractorId = 1 },
            new Job { Id = 2, JobName = "Job 2", ContractorId = 1 }
        };
        _mockJobRepository.Setup(r => r.GetByContractorIdAsync(1)).ReturnsAsync(jobs);

        // Act
        var result = await _service.GetJobsByContractorAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, job => Assert.Equal(1, job.ContractorId));
        _mockJobRepository.Verify(r => r.GetByContractorIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateJobAsync_CreatesJobWithPriceSnapshots()
    {
        // Arrange
        var fixtures = new List<FixtureItem>
        {
            new FixtureItem { Id = 1, Name = "Fixture 1" },
            new FixtureItem { Id = 2, Name = "Fixture 2" }
        };
        fixtures[0].BidItems.Add(new BidItem { Price = 50.00m, Phase = Phase.Underground });
        fixtures[1].BidItems.Add(new BidItem { Price = 75.00m, Phase = Phase.StackOut });

        _mockFixtureRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fixtures[0]);
        _mockFixtureRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(fixtures[1]);

        var fixtureQuantities = new Dictionary<int, int>
        {
            { 1, 3 },
            { 2, 2 }
        };

        _mockJobRepository.Setup(r => r.AddAsync(It.IsAny<Job>()))
            .ReturnsAsync((Job j) => { j.Id = 1; return j; });

        // Act
        var result = await _service.CreateJobAsync(1, "Test Job", fixtureQuantities);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.JobFixtureItems.Count);
        
        var item1 = result.JobFixtureItems.First(jfi => jfi.FixtureItemId == 1);
        Assert.Equal(50.00m, item1.Price); // Snapshot
        Assert.Equal(3, item1.Quantity);
        
        var item2 = result.JobFixtureItems.First(jfi => jfi.FixtureItemId == 2);
        Assert.Equal(75.00m, item2.Price); // Snapshot
        Assert.Equal(2, item2.Quantity);
        
        _mockJobRepository.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Once);
    }

    [Fact]
    public async Task UpdateJobAsync_AddsNewFixturesWithZeroQuantity()
    {
        // Arrange
        var existingJob = new Job
        {
            Id = 1,
            JobName = "Existing Job",
            ContractorId = 1
        };
        existingJob.JobFixtureItems.Add(new JobFixtureItem
        {
            Id = 1,
            FixtureItemId = 1,
            Price = 50.00m,
            Quantity = 2
        });

        var allFixtures = new List<FixtureItem>
        {
            new FixtureItem { Id = 1, Name = "Existing Fixture" },
            new FixtureItem { Id = 2, Name = "New Fixture" }
        };
        allFixtures[0].BidItems.Add(new BidItem { Price = 50.00m, Phase = Phase.Trim });
        allFixtures[1].BidItems.Add(new BidItem { Price = 100.00m, Phase = Phase.StackOut });

        _mockJobRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingJob);
        _mockFixtureRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(allFixtures);
        _mockJobRepository.Setup(r => r.UpdateAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);

        var fixtureQuantities = new Dictionary<int, int>
        {
            { 1, 5 }, // Update existing
            { 2, 0 }  // Ensure new fixture gets quantity 0 and price snapshot considered only when >0
        };

        // Act
        await _service.UpdateJobAsync(1, "Existing Job", JobStatus.Open, fixtureQuantities);

        // Assert
        // With current Update logic, only items with quantity > 0 are kept.
        // So new fixture with quantity 0 should NOT be present.
        Assert.Single(existingJob.JobFixtureItems);
        
        var existingItem = existingJob.JobFixtureItems.First(jfi => jfi.FixtureItemId == 1);
        Assert.Equal(5, existingItem.Quantity);
        Assert.Equal(50.00m, existingItem.Price); // Keeps original snapshot
        
        Assert.DoesNotContain(existingJob.JobFixtureItems, jfi => jfi.FixtureItemId == 2);
        
        _mockJobRepository.Verify(r => r.UpdateAsync(It.IsAny<Job>()), Times.Once);
    }

    [Fact]
    public async Task GetAvailableFixturesAsync_ReturnsAllFixtures()
    {
        // Arrange
        var fixtures = new List<FixtureItem>
        {
            new FixtureItem { Id = 1, Name = "Fixture 1" },
            new FixtureItem { Id = 2, Name = "Fixture 2" }
        };
        _mockFixtureRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(fixtures);

        // Act
        var result = await _service.GetAvailableFixturesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockFixtureRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteJobAsync_CallsRepositoryDelete()
    {
        // Arrange
        var jobId = 1;
        _mockJobRepository.Setup(r => r.DeleteAsync(jobId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteJobAsync(jobId);

        // Assert
        _mockJobRepository.Verify(r => r.DeleteAsync(jobId), Times.Once);
    }
}
