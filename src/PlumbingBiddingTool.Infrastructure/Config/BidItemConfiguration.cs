using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class BidItemConfiguration : IEntityTypeConfiguration<BidItem>
{
    public void Configure(EntityTypeBuilder<BidItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.Phase).HasConversion<int>();
    }
}
