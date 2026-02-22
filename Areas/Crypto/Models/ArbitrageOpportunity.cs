namespace NavegaStudio.Areas.Crypto.Models;

public class ArbitrageOpportunity
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string BuyExchange { get; set; } = string.Empty;
    public string SellExchange { get; set; } = string.Empty;
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal SpreadAmount { get; set; }
    public decimal SpreadPercent { get; set; }
    public decimal EstimatedProfit { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
