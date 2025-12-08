using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class JobOptionConfiguration : IEntityTypeConfiguration<JobOption>
{
    public void Configure(EntityTypeBuilder<JobOption> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
    }
}
