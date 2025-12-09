using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.Jobs;

public class JobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IFixtureItemRepository _fixtureItemRepository;
    private readonly IJobOptionRepository _jobOptionRepository;

    public JobService(IJobRepository jobRepository, IFixtureItemRepository fixtureItemRepository, IJobOptionRepository jobOptionRepository)
    {
        _jobRepository = jobRepository;
        _fixtureItemRepository = fixtureItemRepository;
        _jobOptionRepository = jobOptionRepository;
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

    public async Task<Job> CreateJobAsync(int contractorId, string jobName, Dictionary<int, int> fixtureQuantities, IEnumerable<JobOptionInput> jobOptions)
    {
        var job = new Job
        {
            ContractorId = contractorId,
            JobName = jobName,
            Status = JobStatus.Open
        };

        var optionItems = jobOptions
            .Where(o => !string.IsNullOrWhiteSpace(o.Name) && o.Quantity > 0)
            .Select(o => new JobOption
            {
                Name = o.Name.Trim(),
                Quantity = o.Quantity,
                Price = o.Price
            })
            .ToList();

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
        job.JobOptions = optionItems;
        return await _jobRepository.AddAsync(job);
    }

    public async Task UpdateJobAsync(int jobId, string jobName, JobStatus status, Dictionary<int, int> fixtureQuantities, IEnumerable<JobOptionInput> jobOptions)
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

        // Replace job options with provided list
        var existingOptions = (await _jobOptionRepository.GetByJobIdAsync(jobId)).ToList();
        foreach (var option in existingOptions)
        {
            await _jobOptionRepository.DeleteAsync(option.Id);
        }

        var newOptions = new List<JobOption>();
        foreach (var option in jobOptions.Where(o => !string.IsNullOrWhiteSpace(o.Name) && o.Quantity > 0))
        {
            var created = await _jobOptionRepository.AddAsync(new JobOption
            {
                JobId = jobId,
                Name = option.Name.Trim(),
                Quantity = option.Quantity,
                Price = option.Price
            });
            newOptions.Add(created);
        }

        job.JobOptions = newOptions;
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
