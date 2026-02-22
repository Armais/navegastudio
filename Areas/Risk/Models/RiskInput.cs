using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Risk.Models;

public class RiskInput
{
    public string Mode { get; set; } = "RiskCalculator";

    [Required]
    [Range(100, 100_000_000, ErrorMessage = "Account capital must be between $100 and $100,000,000")]
    public decimal AccountCapital { get; set; } = 10000m;

    [Range(0.1, 100, ErrorMessage = "Risk percentage must be between 0.1% and 100%")]
    public decimal? RiskPercent { get; set; }

    [Required]
    [Range(0.0001, 1_000_000, ErrorMessage = "Entry price must be positive")]
    public decimal EntryPrice { get; set; }

    [Required]
    [Range(0.0001, 1_000_000, ErrorMessage = "Stop loss must be positive")]
    public decimal StopLoss { get; set; }

    [Range(0.0001, 1_000_000)]
    public decimal? TakeProfit { get; set; }

    [Range(1, 10_000_000, ErrorMessage = "Position size must be between 1 and 10,000,000")]
    public decimal? PositionSize { get; set; }

    [Range(0, 100)]
    public decimal CommissionPercent { get; set; } = 0.1m;

    public string TradeDirection { get; set; } = "Long";
}
