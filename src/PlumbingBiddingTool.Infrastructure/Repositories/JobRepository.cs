using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;

namespace PlumbingBiddingTool.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(j => j.Contractor)
            .Include(j => j.JobFixtureItems)
                .ThenInclude(jfi => jfi.FixtureItem)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetByContractorIdAsync(int contractorId)
    {
        return await _context.Jobs
            .Include(j => j.Contractor)
            .Include(j => j.JobFixtureItems)
                .ThenInclude(jfi => jfi.FixtureItem)
            .Where(j => j.ContractorId == contractorId)
            .ToListAsync();
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        return await _context.Jobs
            .Include(j => j.Contractor)
            .Include(j => j.JobFixtureItems)
                .ThenInclude(jfi => jfi.FixtureItem)
                    .ThenInclude(fi => fi.BidItems)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<Job> AddAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task UpdateAsync(Job job)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job != null)
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }
    }
}
