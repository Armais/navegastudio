using NavegaStudio.Areas.Backtesting.Models;
using Microsoft.EntityFrameworkCore;

namespace NavegaStudio.Data;

public class BacktestDbContext : DbContext
{
    public BacktestDbContext(DbContextOptions<BacktestDbContext> options)
        : base(options) { }

    public DbSet<HistoricalPrice> HistoricalPrices => Set<HistoricalPrice>();
    public DbSet<BacktestResult> BacktestResults => Set<BacktestResult>();
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<EquityPoint> EquityPoints => Set<EquityPoint>();
    public DbSet<DrawdownPoint> DrawdownPoints => Set<DrawdownPoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistoricalPrice>(entity =>
        {
            entity.HasIndex(e => new { e.Symbol, e.Date }).IsUnique();
            entity.Property(e => e.Open).HasPrecision(18, 4);
            entity.Property(e => e.High).HasPrecision(18, 4);
            entity.Property(e => e.Low).HasPrecision(18, 4);
            entity.Property(e => e.Close).HasPrecision(18, 4);
        });

        modelBuilder.Entity<BacktestResult>(entity =>
        {
            entity.HasIndex(e => e.Symbol);
            entity.Property(e => e.InitialCapital).HasPrecision(18, 2);
            entity.Property(e => e.FinalCapital).HasPrecision(18, 2);
            entity.Property(e => e.TotalReturn).HasPrecision(18, 4);
            entity.Property(e => e.MaxDrawdown).HasPrecision(18, 4);
            entity.Property(e => e.WinRate).HasPrecision(18, 4);
            entity.Property(e => e.SharpeRatio).HasPrecision(18, 4);
            entity.Property(e => e.SortinoRatio).HasPrecision(18, 4);
            entity.Property(e => e.ProfitFactor).HasPrecision(18, 4);
            entity.Property(e => e.Expectancy).HasPrecision(18, 4);
            entity.Property(e => e.AvgWin).HasPrecision(18, 2);
            entity.Property(e => e.AvgLoss).HasPrecision(18, 2);
            entity.Property(e => e.TotalCommissions).HasPrecision(18, 2);
            entity.Property(e => e.BenchmarkReturn).HasPrecision(18, 4);
            entity.Property(e => e.BenchmarkFinalCapital).HasPrecision(18, 2);
            entity.Property(e => e.StrategyDescription).HasMaxLength(200);
            entity.HasMany(e => e.Trades).WithOne().HasForeignKey(t => t.BacktestResultId);
            entity.HasMany(e => e.EquityCurve).WithOne().HasForeignKey(t => t.BacktestResultId);
            entity.HasMany(e => e.DrawdownCurve).WithOne().HasForeignKey(t => t.BacktestResultId);
        });

        modelBuilder.Entity<Trade>(entity =>
        {
            entity.Property(e => e.EntryPrice).HasPrecision(18, 4);
            entity.Property(e => e.ExitPrice).HasPrecision(18, 4);
            entity.Property(e => e.Quantity).HasPrecision(18, 6);
            entity.Property(e => e.ProfitLoss).HasPrecision(18, 2);
            entity.Property(e => e.ProfitLossPercent).HasPrecision(18, 4);
            entity.Property(e => e.ExitReason).HasMaxLength(50);
        });

        modelBuilder.Entity<EquityPoint>(entity =>
        {
            entity.Property(e => e.Equity).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DrawdownPoint>(entity =>
        {
            entity.Property(e => e.DrawdownPct).HasPrecision(18, 4);
        });
    }
}
