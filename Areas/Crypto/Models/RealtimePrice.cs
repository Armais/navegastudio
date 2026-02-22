namespace NavegaStudio.Areas.Crypto.Models;

public class RealtimePrice
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
    public decimal Change24hPercent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PriceSnapshot
{
    public string Symbol { get; set; } = string.Empty;
    public List<ExchangePrice> Exchanges { get; set; } = new();
    public decimal? SpreadPercent { get; set; }
}

public class ExchangePrice
{
    public string Exchange { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
    public decimal Change24hPercent { get; set; }
    public DateTime Timestamp { get; set; }
}
