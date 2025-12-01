using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.BidItems;

public class BidItemService
{
    private readonly IBidItemRepository _repository;

    public BidItemService(IBidItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BidItem>> GetAllBidItemsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<BidItem?> GetBidItemByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<BidItem> CreateBidItemAsync(string name, decimal price)
    {
        var bidItem = new BidItem
        {
            Name = name,
            Price = price
        };

        return await _repository.AddAsync(bidItem);
    }

    public async Task UpdateBidItemAsync(int id, string name, decimal price)
    {
        var bidItem = await _repository.GetByIdAsync(id);
        if (bidItem != null)
        {
            bidItem.Name = name;
            bidItem.Price = price;
            await _repository.UpdateAsync(bidItem);
        }
    }

    public async Task DeleteBidItemAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
