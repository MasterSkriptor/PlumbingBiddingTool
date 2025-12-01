using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Domain.Repositories;

public interface IBidItemRepository
{
    Task<IEnumerable<BidItem>> GetAllAsync();
    Task<BidItem?> GetByIdAsync(int id);
    Task<BidItem> AddAsync(BidItem bidItem);
    Task UpdateAsync(BidItem bidItem);
    Task DeleteAsync(int id);
}
