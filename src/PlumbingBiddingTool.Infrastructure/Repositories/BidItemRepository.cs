using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;

namespace PlumbingBiddingTool.Infrastructure.Repositories;

public class BidItemRepository : IBidItemRepository
{
    private readonly ApplicationDbContext _context;

    public BidItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BidItem>> GetAllAsync()
    {
        return await _context.BidItems.ToListAsync();
    }

    public async Task<BidItem?> GetByIdAsync(int id)
    {
        return await _context.BidItems.FindAsync(id);
    }

    public async Task<BidItem> AddAsync(BidItem bidItem)
    {
        _context.BidItems.Add(bidItem);
        await _context.SaveChangesAsync();
        return bidItem;
    }

    public async Task UpdateAsync(BidItem bidItem)
    {
        _context.BidItems.Update(bidItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var bidItem = await _context.BidItems.FindAsync(id);
        if (bidItem != null)
        {
            _context.BidItems.Remove(bidItem);
            await _context.SaveChangesAsync();
        }
    }
}
