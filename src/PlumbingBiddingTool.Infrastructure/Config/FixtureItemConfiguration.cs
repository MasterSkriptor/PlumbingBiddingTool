using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class FixtureItemConfiguration : IEntityTypeConfiguration<FixtureItem>
{
    public void Configure(EntityTypeBuilder<FixtureItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        
        builder.HasMany(f => f.BidItems)
            .WithMany()
            .UsingEntity(j => j.ToTable("FixtureItemBidItems"));
    }
}
