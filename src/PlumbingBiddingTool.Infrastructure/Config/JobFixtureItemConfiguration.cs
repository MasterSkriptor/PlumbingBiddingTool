using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class JobFixtureItemConfiguration : IEntityTypeConfiguration<JobFixtureItem>
{
    public void Configure(EntityTypeBuilder<JobFixtureItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
        
        builder.HasOne(jfi => jfi.FixtureItem)
            .WithMany()
            .HasForeignKey(jfi => jfi.FixtureItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
