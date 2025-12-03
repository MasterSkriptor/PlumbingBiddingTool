using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.Jobs;

public class JobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IFixtureItemRepository _fixtureItemRepository;

    public JobService(IJobRepository jobRepository, IFixtureItemRepository fixtureItemRepository)
    {
        _jobRepository = jobRepository;
        _fixtureItemRepository = fixtureItemRepository;
    }

    public async Task<IEnumerable<Job>> GetAllJobsAsync()
    {
        return await _jobRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Job>> GetJobsByContractorAsync(int contractorId)
    {
        return await _jobRepository.GetByContractorIdAsync(contractorId);
    }

    public async Task<Job?> GetJobByIdAsync(int id)
    {
        return await _jobRepository.GetByIdAsync(id);
    }

    public async Task<Job> CreateJobAsync(int contractorId, string jobName, Dictionary<int, int> fixtureQuantities)
    {
        var job = new Job
        {
            ContractorId = contractorId,
            JobName = jobName,
            Status = JobStatus.Open
        };

        // Create JobFixtureItems with price snapshots
        var jobFixtureItems = new List<JobFixtureItem>();
        foreach (var (fixtureId, quantity) in fixtureQuantities)
        {
            if (quantity > 0) // Only add items with quantity > 0
            {
                var fixture = await _fixtureItemRepository.GetByIdAsync(fixtureId);
                if (fixture != null)
                {
                    jobFixtureItems.Add(new JobFixtureItem
                    {
                        FixtureItemId = fixtureId,
                        Quantity = quantity,
                        Price = fixture.Price // Snapshot current price
                    });
                }
            }
        }

        job.JobFixtureItems = jobFixtureItems;
        return await _jobRepository.AddAsync(job);
    }

    public async Task UpdateJobAsync(int jobId, string jobName, JobStatus status, Dictionary<int, int> fixtureQuantities)
    {
        var job = await _jobRepository.GetByIdAsync(jobId);
        if (job == null) return;

        job.JobName = jobName;
        job.Status = status;

        // Get all available fixtures
        var allFixtures = (await _fixtureItemRepository.GetAllAsync()).ToList();
        
        // Update existing JobFixtureItems and add new ones
        var updatedItems = new List<JobFixtureItem>();

        foreach (var fixture in allFixtures)
        {
            var quantity = fixtureQuantities.GetValueOrDefault(fixture.Id, 0);
            
            // Find existing JobFixtureItem
            var existingItem = job.JobFixtureItems.FirstOrDefault(jfi => jfi.FixtureItemId == fixture.Id);
            
            if (existingItem != null)
            {
                // Update existing item
                existingItem.Quantity = quantity;
                if (quantity > 0)
                {
                    updatedItems.Add(existingItem);
                }
            }
            else if (quantity > 0)
            {
                // Add new item with current price snapshot
                updatedItems.Add(new JobFixtureItem
                {
                    JobId = jobId,
                    FixtureItemId = fixture.Id,
                    Quantity = quantity,
                    Price = fixture.Price
                });
            }
        }

        job.JobFixtureItems = updatedItems;
        await _jobRepository.UpdateAsync(job);
    }

    public async Task DeleteJobAsync(int id)
    {
        await _jobRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<FixtureItem>> GetAvailableFixturesAsync()
    {
        return await _fixtureItemRepository.GetAllAsync();
    }
}
