using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.JobName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Status).HasConversion<int>();
        
        builder.HasMany(j => j.JobFixtureItems)
            .WithOne(jfi => jfi.Job)
            .HasForeignKey(jfi => jfi.JobId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(j => j.JobOptions)
            .WithOne(jo => jo.Job)
            .HasForeignKey(jo => jo.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
