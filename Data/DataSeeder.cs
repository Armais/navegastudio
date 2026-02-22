using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Data;

public static class DataSeeder
{
    private record SymbolConfig(string Symbol, decimal BasePrice, double Volatility, int MinVolume, int MaxVolume, bool SkipWeekends);

    public static async Task SeedAsync(BacktestDbContext context)
    {
        if (context.HistoricalPrices.Any()) return;

        var random = new Random(42);

        var symbols = new List<SymbolConfig>
        {
            new("AAPL", 150m, 0.03, 5_000_000, 50_000_000, true),
            new("MSFT", 300m, 0.03, 5_000_000, 50_000_000, true),
            new("GOOGL", 130m, 0.03, 5_000_000, 50_000_000, true),
            new("TSLA", 250m, 0.03, 5_000_000, 50_000_000, true),
            new("AMZN", 140m, 0.03, 5_000_000, 50_000_000, true),
            new("META", 350m, 0.03, 5_000_000, 50_000_000, true),
            new("NVDA", 450m, 0.03, 5_000_000, 50_000_000, true),
            new("NFLX", 400m, 0.03, 5_000_000, 50_000_000, true),
            new("JPM", 170m, 0.03, 5_000_000, 50_000_000, true),
            new("BA", 200m, 0.03, 5_000_000, 50_000_000, true),
            new("DIS", 100m, 0.03, 5_000_000, 50_000_000, true),
            new("V", 260m, 0.03, 5_000_000, 50_000_000, true),
            new("WMT", 160m, 0.03, 5_000_000, 50_000_000, true),
            new("KO", 60m, 0.03, 5_000_000, 50_000_000, true),
            new("INTC", 40m, 0.03, 5_000_000, 50_000_000, true),

            new("BTC", 42000m, 0.05, 1_000_000, 20_000_000, false),
            new("ETH", 2200m, 0.05, 1_000_000, 20_000_000, false),
            new("SOL", 95m, 0.05, 1_000_000, 20_000_000, false),
            new("XRP", 0.55m, 0.05, 1_000_000, 20_000_000, false),
            new("ADA", 0.45m, 0.05, 1_000_000, 20_000_000, false),
            new("DOGE", 0.08m, 0.05, 1_000_000, 20_000_000, false),

            new("EUR/USD", 1.0850m, 0.005, 100_000, 500_000, true),
            new("GBP/USD", 1.2650m, 0.005, 100_000, 500_000, true),
            new("USD/JPY", 148.50m, 0.005, 100_000, 500_000, true),
            new("AUD/USD", 0.6550m, 0.005, 100_000, 500_000, true),
            new("USD/CHF", 0.8750m, 0.005, 100_000, 500_000, true),
        };

        foreach (var config in symbols)
        {
            var prices = GeneratePriceData(config, 300, random);
            context.HistoricalPrices.AddRange(prices);
        }

        await context.SaveChangesAsync();
    }

    private static List<HistoricalPrice> GeneratePriceData(
        SymbolConfig config, int days, Random random)
    {
        var prices = new List<HistoricalPrice>();
        decimal price = config.BasePrice;
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            if (config.SkipWeekends &&
                (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                continue;

            double vol = config.Volatility;
            decimal dailyReturn = (decimal)(random.NextDouble() * vol * 2 - vol * 0.933);
            decimal open = price;
            decimal close = price * (1 + dailyReturn);
            decimal high = Math.Max(open, close) * (1 + (decimal)(random.NextDouble() * 0.015));
            decimal low = Math.Min(open, close) * (1 - (decimal)(random.NextDouble() * 0.015));
            long volume = random.Next(config.MinVolume, config.MaxVolume);

            prices.Add(new HistoricalPrice
            {
                Symbol = config.Symbol,
                Date = date,
                Open = Math.Round(open, 4),
                High = Math.Round(high, 4),
                Low = Math.Round(low, 4),
                Close = Math.Round(close, 4),
                Volume = volume
            });

            price = close;
        }

        return prices;
    }
}
