using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;

namespace PlumbingBiddingTool.Infrastructure.Repositories;

public class FixtureItemRepository : IFixtureItemRepository
{
    private readonly ApplicationDbContext _context;

    public FixtureItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FixtureItem>> GetAllAsync()
    {
        return await _context.FixtureItems
            .Include(f => f.BidItems)
            .ToListAsync();
    }

    public async Task<FixtureItem?> GetByIdAsync(int id)
    {
        return await _context.FixtureItems
            .Include(f => f.BidItems)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<FixtureItem> AddAsync(FixtureItem fixtureItem)
    {
        _context.FixtureItems.Add(fixtureItem);
        await _context.SaveChangesAsync();
        return fixtureItem;
    }

    public async Task UpdateAsync(FixtureItem fixtureItem)
    {
        _context.FixtureItems.Update(fixtureItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var fixtureItem = await _context.FixtureItems.FindAsync(id);
        if (fixtureItem != null)
        {
            _context.FixtureItems.Remove(fixtureItem);
            await _context.SaveChangesAsync();
        }
    }
}
