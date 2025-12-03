using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;

namespace PlumbingBiddingTool.Infrastructure.Repositories;

public class ContractorRepository : IContractorRepository
{
    private readonly ApplicationDbContext _context;

    public ContractorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contractor>> GetAllAsync()
    {
        return await _context.Contractors
            .Include(c => c.Jobs)
            .ToListAsync();
    }

    public async Task<Contractor?> GetByIdAsync(int id)
    {
        return await _context.Contractors
            .Include(c => c.Jobs)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contractor> AddAsync(Contractor contractor)
    {
        _context.Contractors.Add(contractor);
        await _context.SaveChangesAsync();
        return contractor;
    }

    public async Task UpdateAsync(Contractor contractor)
    {
        _context.Contractors.Update(contractor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contractor = await _context.Contractors.FindAsync(id);
        if (contractor != null)
        {
            _context.Contractors.Remove(contractor);
            await _context.SaveChangesAsync();
        }
    }
}
