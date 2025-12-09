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

    public async Task<BidItem> CreateBidItemAsync(string name, decimal price, Phase phase, ItemType itemType)
    {
        var bidItem = new BidItem
        {
            Name = name,
            Price = price,
            Phase = phase,
            ItemType = itemType
        };

        return await _repository.AddAsync(bidItem);
    }

    public async Task UpdateBidItemAsync(int id, string name, decimal price, Phase phase, ItemType itemType)
    {
        var bidItem = await _repository.GetByIdAsync(id);
        if (bidItem != null)
        {
            bidItem.Name = name;
            bidItem.Price = price;
            bidItem.Phase = phase;
            bidItem.ItemType = itemType;
            await _repository.UpdateAsync(bidItem);
        }
    }

    public async Task DeleteBidItemAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
