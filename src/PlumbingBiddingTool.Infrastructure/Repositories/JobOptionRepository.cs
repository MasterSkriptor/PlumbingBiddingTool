using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;
using PlumbingBiddingTool.Domain.Repositories;
using PlumbingBiddingTool.Infrastructure.Data;

namespace PlumbingBiddingTool.Infrastructure.Repositories;

public class JobOptionRepository : IJobOptionRepository
{
    private readonly ApplicationDbContext _context;

    public JobOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobOption>> GetAllAsync()
    {
        return await _context.JobOptions.ToListAsync();
    }

    public async Task<JobOption?> GetByIdAsync(int id)
    {
        return await _context.JobOptions.FindAsync(id);
    }

    public async Task<IEnumerable<JobOption>> GetByJobIdAsync(int jobId)
    {
        return await _context.JobOptions.Where(jo => jo.JobId == jobId).ToListAsync();
    }

    public async Task<JobOption> AddAsync(JobOption jobOption)
    {
        _context.JobOptions.Add(jobOption);
        await _context.SaveChangesAsync();
        return jobOption;
    }

    public async Task UpdateAsync(JobOption jobOption)
    {
        _context.JobOptions.Update(jobOption);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var jobOption = await _context.JobOptions.FindAsync(id);
        if (jobOption != null)
        {
            _context.JobOptions.Remove(jobOption);
            await _context.SaveChangesAsync();
        }
    }
}
