using System.ComponentModel.DataAnnotations.Schema;

namespace NavegaStudio.Areas.Backtesting.Models;

public class BacktestResult
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal InitialCapital { get; set; }
    public decimal FinalCapital { get; set; }
    public decimal TotalReturn { get; set; }
    public decimal MaxDrawdown { get; set; }
    public int TotalTrades { get; set; }
    public int WinningTrades { get; set; }
    public int LosingTrades { get; set; }
    public decimal WinRate { get; set; }
    public decimal SharpeRatio { get; set; }
    public decimal SortinoRatio { get; set; }
    public decimal ProfitFactor { get; set; }
    public decimal Expectancy { get; set; }
    public decimal AvgWin { get; set; }
    public decimal AvgLoss { get; set; }
    public int MaxConsecutiveWins { get; set; }
    public int MaxConsecutiveLosses { get; set; }
    public decimal TotalCommissions { get; set; }
    public decimal BenchmarkReturn { get; set; }
    public decimal BenchmarkFinalCapital { get; set; }
    public StrategyType StrategyType { get; set; }
    public string StrategyDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Trade> Trades { get; set; } = new();
    public List<EquityPoint> EquityCurve { get; set; } = new();
    public List<DrawdownPoint> DrawdownCurve { get; set; } = new();

    [NotMapped]
    public List<EquityPoint> BenchmarkCurve { get; set; } = new();
}

public class Trade
{
    public int Id { get; set; }
    public int BacktestResultId { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime ExitDate { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal ExitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercent { get; set; }
    public string Type { get; set; } = "Long";
    public string ExitReason { get; set; } = "Signal";
}

public class EquityPoint
{
    public int Id { get; set; }
    public int BacktestResultId { get; set; }
    public DateTime Date { get; set; }
    public decimal Equity { get; set; }
}

public class DrawdownPoint
{
    public int Id { get; set; }
    public int BacktestResultId { get; set; }
    public DateTime Date { get; set; }
    public decimal DrawdownPct { get; set; }
}
