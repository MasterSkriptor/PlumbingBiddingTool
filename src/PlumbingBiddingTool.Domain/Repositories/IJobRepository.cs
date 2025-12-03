using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Repositories;

public interface IJobRepository
{
    Task<IEnumerable<Job>> GetAllAsync();
    Task<IEnumerable<Job>> GetByContractorIdAsync(int contractorId);
    Task<Job?> GetByIdAsync(int id);
    Task<Job> AddAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(int id);
}
