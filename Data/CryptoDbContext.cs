using NavegaStudio.Areas.Crypto.Models;
using Microsoft.EntityFrameworkCore;

namespace NavegaStudio.Data;

public class CryptoDbContext : DbContext
{
    public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options) { }

    public DbSet<RealtimePrice> RealtimePrices => Set<RealtimePrice>();
    public DbSet<ArbitrageOpportunity> ArbitrageOpportunities => Set<ArbitrageOpportunity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RealtimePrice>(entity =>
        {
            entity.HasIndex(e => new { e.Symbol, e.Exchange, e.Timestamp });
            entity.Property(e => e.Price).HasPrecision(18, 8);
            entity.Property(e => e.Bid).HasPrecision(18, 8);
            entity.Property(e => e.Ask).HasPrecision(18, 8);
            entity.Property(e => e.Volume24h).HasPrecision(18, 2);
            entity.Property(e => e.Change24h).HasPrecision(18, 8);
            entity.Property(e => e.Change24hPercent).HasPrecision(18, 4);
        });

        modelBuilder.Entity<ArbitrageOpportunity>(entity =>
        {
            entity.HasIndex(e => new { e.Symbol, e.DetectedAt });
            entity.Property(e => e.BuyPrice).HasPrecision(18, 8);
            entity.Property(e => e.SellPrice).HasPrecision(18, 8);
            entity.Property(e => e.SpreadAmount).HasPrecision(18, 8);
            entity.Property(e => e.SpreadPercent).HasPrecision(18, 4);
            entity.Property(e => e.EstimatedProfit).HasPrecision(18, 2);
        });
    }
}
