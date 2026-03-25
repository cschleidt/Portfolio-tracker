using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations;

public class PortfolioChangeConfiguration : IEntityTypeConfiguration<PortfolioChange>
{
    public void Configure(EntityTypeBuilder<PortfolioChange> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ticker).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.OldValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.NewValue).HasColumnType("decimal(18,4)");
        builder.Property(x => x.ChangeType).HasConversion<string>();

        builder.HasIndex(x => x.DetectedAt);
    }
}
