using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Backtesting.Models;

public class BacktestRequest
{
    [Required(ErrorMessage = "Symbol is required")]
    [MaxLength(10)]
    public string Symbol { get; set; } = "AAPL";

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-180);

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Range(100, 10_000_000, ErrorMessage = "Capital must be between 100 and 10,000,000")]
    public decimal InitialCapital { get; set; } = 10000m;

    [Required]
    public StrategyType StrategyType { get; set; } = StrategyType.SmaCrossover;

    public StrategyParameters Parameters { get; set; } = new();

    [Range(0.1, 100, ErrorMessage = "Risk per trade must be between 0.1% and 100%")]
    public decimal RiskPerTrade { get; set; } = 100m;

    [Range(0, 1000, ErrorMessage = "Commission must be between $0 and $1,000")]
    public decimal CommissionPerTrade { get; set; } = 0m;

    [Range(0, 5, ErrorMessage = "Slippage must be between 0% and 5%")]
    public decimal SlippagePct { get; set; } = 0m;

    [Range(0, 50, ErrorMessage = "Stop-loss must be between 0% and 50%")]
    public decimal? StopLossPct { get; set; }

    [Range(0, 200, ErrorMessage = "Take-profit must be between 0% and 200%")]
    public decimal? TakeProfitPct { get; set; }
}
