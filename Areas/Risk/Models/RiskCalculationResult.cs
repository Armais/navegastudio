namespace NavegaStudio.Areas.Risk.Models;

public class RiskCalculationResult
{
    public string Mode { get; set; } = "PositionSizer";
    public decimal RiskPercent { get; set; }
    public decimal PositionSize { get; set; }
    public decimal PositionValue { get; set; }
    public decimal RiskAmount { get; set; }
    public decimal StopLossDistance { get; set; }
    public decimal StopLossPercent { get; set; }
    public decimal MaxLoss { get; set; }
    public decimal? TakeProfitDistance { get; set; }
    public decimal? TakeProfitPercent { get; set; }
    public decimal? PotentialProfit { get; set; }
    public decimal? RiskRewardRatio { get; set; }
    public decimal CommissionCost { get; set; }
    public decimal NetMaxLoss { get; set; }
    public decimal? NetPotentialProfit { get; set; }
    public decimal Leverage { get; set; }
    public decimal PortfolioRiskPercent { get; set; }
    public string TradeDirection { get; set; } = "Long";
    public decimal? KellyPercent { get; set; }
    public decimal? KellyPositionSize { get; set; }
}
