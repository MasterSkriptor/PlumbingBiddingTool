using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlumbingBiddingTool.Domain.Entities;

namespace PlumbingBiddingTool.Infrastructure.Config;

public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        
        builder.HasMany(c => c.Jobs)
            .WithOne(j => j.Contractor)
            .HasForeignKey(j => j.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
