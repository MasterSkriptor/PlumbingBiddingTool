using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.Contractors;

public class ContractorService
{
    private readonly IContractorRepository _repository;

    public ContractorService(IContractorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Contractor>> GetAllContractorsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Contractor?> GetContractorByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Contractor> CreateContractorAsync(string name)
    {
        var contractor = new Contractor
        {
            Name = name
        };

        return await _repository.AddAsync(contractor);
    }

    public async Task UpdateContractorAsync(int id, string name)
    {
        var contractor = await _repository.GetByIdAsync(id);
        if (contractor != null)
        {
            contractor.Name = name;
            await _repository.UpdateAsync(contractor);
        }
    }

    public async Task DeleteContractorAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
