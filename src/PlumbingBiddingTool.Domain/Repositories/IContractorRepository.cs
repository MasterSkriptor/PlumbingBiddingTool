using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Repositories;

public interface IContractorRepository
{
    Task<IEnumerable<Contractor>> GetAllAsync();
    Task<Contractor?> GetByIdAsync(int id);
    Task<Contractor> AddAsync(Contractor contractor);
    Task UpdateAsync(Contractor contractor);
    Task DeleteAsync(int id);
}
