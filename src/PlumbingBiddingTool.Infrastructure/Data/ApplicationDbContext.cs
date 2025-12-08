using Microsoft.EntityFrameworkCore;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BidItem> BidItems => Set<BidItem>();

    public DbSet<FixtureItem> FixtureItems => Set<FixtureItem>();

    public DbSet<Contractor> Contractors => Set<Contractor>();

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<JobFixtureItem> JobFixtureItems => Set<JobFixtureItem>();

    public DbSet<JobOption> JobOptions => Set<JobOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the Config folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
