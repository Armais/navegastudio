using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Backtesting.Models;

public class HistoricalPrice
{
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Symbol { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}
