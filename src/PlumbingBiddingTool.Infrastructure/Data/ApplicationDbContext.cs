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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BidItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<FixtureItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            
            entity.HasMany(f => f.BidItems)
                .WithMany()
                .UsingEntity(j => j.ToTable("FixtureItemBidItems"));
        });

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            
            entity.HasMany(c => c.Jobs)
                .WithOne(j => j.Contractor)
                .HasForeignKey(j => j.ContractorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
            
            entity.HasMany(j => j.JobFixtureItems)
                .WithOne(jfi => jfi.Job)
                .HasForeignKey(jfi => jfi.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobFixtureItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
            
            entity.HasOne(jfi => jfi.FixtureItem)
                .WithMany()
                .HasForeignKey(jfi => jfi.FixtureItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
