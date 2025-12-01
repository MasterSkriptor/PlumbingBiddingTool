using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Repositories;

public interface IFixtureItemRepository
{
    Task<IEnumerable<FixtureItem>> GetAllAsync();
    Task<FixtureItem?> GetByIdAsync(int id);
    Task<FixtureItem> AddAsync(FixtureItem fixtureItem);
    Task UpdateAsync(FixtureItem fixtureItem);
    Task DeleteAsync(int id);
}
