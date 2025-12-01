using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;

namespace PlumbingBiddingTool.Application.FixtureItems;

public class FixtureItemService
{
    private readonly IFixtureItemRepository _fixtureRepository;
    private readonly IBidItemRepository _bidItemRepository;

    public FixtureItemService(IFixtureItemRepository fixtureRepository, IBidItemRepository bidItemRepository)
    {
        _fixtureRepository = fixtureRepository;
        _bidItemRepository = bidItemRepository;
    }

    public async Task<IEnumerable<FixtureItem>> GetAllFixtureItemsAsync()
    {
        return await _fixtureRepository.GetAllAsync();
    }

    public async Task<FixtureItem?> GetFixtureItemByIdAsync(int id)
    {
        return await _fixtureRepository.GetByIdAsync(id);
    }

    public async Task<FixtureItem> CreateFixtureItemAsync(string name, List<int> bidItemIds)
    {
        var fixtureItem = new FixtureItem
        {
            Name = name
        };

        // Load the actual BidItems from the database
        if (bidItemIds != null && bidItemIds.Any())
        {
            var bidItems = new List<BidItem>();
            foreach (var bidItemId in bidItemIds)
            {
                var bidItem = await _bidItemRepository.GetByIdAsync(bidItemId);
                if (bidItem != null)
                {
                    bidItems.Add(bidItem);
                }
            }
            fixtureItem.BidItems = bidItems;
        }

        return await _fixtureRepository.AddAsync(fixtureItem);
    }

    public async Task UpdateFixtureItemAsync(int id, string name, List<int> bidItemIds)
    {
        var fixtureItem = await _fixtureRepository.GetByIdAsync(id);
        if (fixtureItem != null)
        {
            fixtureItem.Name = name;
            
            // Clear existing BidItems and add new ones
            fixtureItem.BidItems.Clear();
            
            if (bidItemIds != null && bidItemIds.Any())
            {
                foreach (var bidItemId in bidItemIds)
                {
                    var bidItem = await _bidItemRepository.GetByIdAsync(bidItemId);
                    if (bidItem != null)
                    {
                        fixtureItem.BidItems.Add(bidItem);
                    }
                }
            }
            
            await _fixtureRepository.UpdateAsync(fixtureItem);
        }
    }

    public async Task DeleteFixtureItemAsync(int id)
    {
        await _fixtureRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BidItem>> GetAvailableBidItemsAsync()
    {
        return await _bidItemRepository.GetAllAsync();
    }
}
