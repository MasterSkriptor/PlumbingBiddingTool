using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Repositories;

public interface IJobOptionRepository
{
    Task<IEnumerable<JobOption>> GetAllAsync();
    Task<JobOption?> GetByIdAsync(int id);
    Task<IEnumerable<JobOption>> GetByJobIdAsync(int jobId);
    Task<JobOption> AddAsync(JobOption jobOption);
    Task UpdateAsync(JobOption jobOption);
    Task DeleteAsync(int id);
}
