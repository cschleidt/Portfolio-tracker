using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.Infrastructure.Persistence.Configurations;

public class HoldingConfiguration : IEntityTypeConfiguration<Holding>
{
    public void Configure(EntityTypeBuilder<Holding> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ticker).IsRequired().HasMaxLength(20);
        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,6)");
        builder.Property(x => x.EntryPrice).HasColumnType("decimal(18,4)");
        builder.Property(x => x.CurrentPrice).HasColumnType("decimal(18,4)");
        builder.Property(x => x.MarketValue).HasColumnType("decimal(18,2)");
        builder.Property(x => x.WeightPercent).HasColumnType("decimal(8,4)");
        builder.Property(x => x.ChangePercent).HasColumnType("decimal(8,4)");
        builder.Property(x => x.Currency).HasMaxLength(10);
    }
}
