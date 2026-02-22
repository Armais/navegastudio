using System.Globalization;
using System.Text.Json;
using NavegaStudio.Areas.Crypto.Models;
using Microsoft.Extensions.Caching.Memory;

namespace NavegaStudio.Areas.Crypto.Services;

public class ExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExchangeService> _logger;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(10);

    private static readonly string[] SupportedSymbols = { "BTC", "ETH", "SOL", "XRP", "ADA", "DOGE" };

    public ExchangeService(HttpClient httpClient, IMemoryCache cache, ILogger<ExchangeService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public string[] GetSupportedSymbols() => SupportedSymbols;

    public async Task<List<PriceSnapshot>> GetAllPricesAsync()
    {
        var snapshots = new List<PriceSnapshot>();
        foreach (var symbol in SupportedSymbols)
        {
            var snapshot = await GetPriceSnapshotAsync(symbol);
            snapshots.Add(snapshot);
        }
        return snapshots;
    }

    public async Task<PriceSnapshot> GetPriceSnapshotAsync(string symbol)
    {
        var cacheKey = $"price_{symbol}";
        if (_cache.TryGetValue<PriceSnapshot>(cacheKey, out var cached) && cached != null)
            return cached;

        var snapshot = new PriceSnapshot { Symbol = symbol };

        var binanceTask = GetBinancePriceAsync(symbol);
        var coinbaseTask = GetCoinbasePriceAsync(symbol);
        var krakenTask = GetKrakenPriceAsync(symbol);

        await Task.WhenAll(binanceTask, coinbaseTask, krakenTask);

        if (binanceTask.Result != null) snapshot.Exchanges.Add(binanceTask.Result);
        if (coinbaseTask.Result != null) snapshot.Exchanges.Add(coinbaseTask.Result);
        if (krakenTask.Result != null) snapshot.Exchanges.Add(krakenTask.Result);

        if (snapshot.Exchanges.Count >= 2)
        {
            var prices = snapshot.Exchanges.Select(e => e.Price).ToList();
            var min = prices.Min();
            var max = prices.Max();
            snapshot.SpreadPercent = min > 0 ? Math.Round((max - min) / min * 100, 4) : 0;
        }

        _cache.Set(cacheKey, snapshot, CacheDuration);
        return snapshot;
    }

    public async Task<List<ArbitrageOpportunity>> DetectArbitrageAsync(decimal minSpreadPercent = 0.1m)
    {
        var opportunities = new List<ArbitrageOpportunity>();
        var snapshots = await GetAllPricesAsync();

        foreach (var snapshot in snapshots)
        {
            if (snapshot.Exchanges.Count < 2) continue;

            var sorted = snapshot.Exchanges.OrderBy(e => e.Price).ToList();
            var cheapest = sorted.First();
            var mostExpensive = sorted.Last();

            decimal spread = mostExpensive.Price - cheapest.Price;
            decimal spreadPct = cheapest.Price > 0 ? spread / cheapest.Price * 100 : 0;

            if (spreadPct >= minSpreadPercent)
            {
                opportunities.Add(new ArbitrageOpportunity
                {
                    Symbol = snapshot.Symbol,
                    BuyExchange = cheapest.Exchange,
                    SellExchange = mostExpensive.Exchange,
                    BuyPrice = cheapest.Price,
                    SellPrice = mostExpensive.Price,
                    SpreadAmount = Math.Round(spread, 8),
                    SpreadPercent = Math.Round(spreadPct, 4),
                    EstimatedProfit = Math.Round(spread * 1, 2)
                });
            }
        }

        return opportunities.OrderByDescending(o => o.SpreadPercent).ToList();
    }

    private async Task<ExchangePrice?> GetBinancePriceAsync(string symbol)
    {
        try
        {
            var pair = $"{symbol}USDT";
            var response = await _httpClient.GetStringAsync(
                $"https://api.binance.com/api/v3/ticker/24hr?symbol={pair}");
            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            return new ExchangePrice
            {
                Exchange = "Binance",
                Price = decimal.Parse(root.GetProperty("lastPrice").GetString()!, CultureInfo.InvariantCulture),
                Bid = decimal.Parse(root.GetProperty("bidPrice").GetString()!, CultureInfo.InvariantCulture),
                Ask = decimal.Parse(root.GetProperty("askPrice").GetString()!, CultureInfo.InvariantCulture),
                Volume24h = decimal.Parse(root.GetProperty("quoteVolume").GetString()!, CultureInfo.InvariantCulture),
                Change24h = decimal.Parse(root.GetProperty("priceChange").GetString()!, CultureInfo.InvariantCulture),
                Change24hPercent = decimal.Parse(root.GetProperty("priceChangePercent").GetString()!, CultureInfo.InvariantCulture),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch Binance price for {Symbol}", symbol);
            return null;
        }
    }

    private async Task<ExchangePrice?> GetCoinbasePriceAsync(string symbol)
    {
        try
        {
            var pair = $"{symbol}-USD";
            var statsResponse = await _httpClient.GetStringAsync(
                $"https://api.exchange.coinbase.com/products/{pair}/stats");
            var stats = JsonDocument.Parse(statsResponse).RootElement;

            decimal price = decimal.Parse(stats.GetProperty("last").GetString()!, CultureInfo.InvariantCulture);
            decimal open = decimal.Parse(stats.GetProperty("open").GetString()!, CultureInfo.InvariantCulture);
            decimal high = decimal.Parse(stats.GetProperty("high").GetString()!, CultureInfo.InvariantCulture);
            decimal low = decimal.Parse(stats.GetProperty("low").GetString()!, CultureInfo.InvariantCulture);
            decimal volume = decimal.Parse(stats.GetProperty("volume").GetString()!, CultureInfo.InvariantCulture);
            decimal change = price - open;

            return new ExchangePrice
            {
                Exchange = "Coinbase",
                Price = price,
                Bid = price,
                Ask = price,
                Volume24h = volume * price,
                Change24h = Math.Round(change, 8),
                Change24hPercent = open > 0 ? Math.Round(change / open * 100, 4) : 0,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch Coinbase price for {Symbol}", symbol);
            return null;
        }
    }

    private async Task<ExchangePrice?> GetKrakenPriceAsync(string symbol)
    {
        try
        {
            var krakenSymbol = symbol switch
            {
                "BTC" => "XXBTZUSD",
                "ETH" => "XETHZUSD",
                "SOL" => "SOLUSD",
                "XRP" => "XXRPZUSD",
                "ADA" => "ADAUSD",
                "DOGE" => "XDGUSD",
                _ => $"{symbol}USD"
            };

            var response = await _httpClient.GetStringAsync(
                $"https://api.kraken.com/0/public/Ticker?pair={krakenSymbol}");
            var json = JsonDocument.Parse(response);
            var result = json.RootElement.GetProperty("result");

            using var enumerator = result.EnumerateObject();
            if (!enumerator.MoveNext()) return null;

            var data = enumerator.Current.Value;
            decimal price = decimal.Parse(data.GetProperty("c")[0].GetString()!, CultureInfo.InvariantCulture);
            decimal open = decimal.Parse(data.GetProperty("o").GetString()!, CultureInfo.InvariantCulture);
            decimal change = price - open;

            return new ExchangePrice
            {
                Exchange = "Kraken",
                Price = price,
                Bid = decimal.Parse(data.GetProperty("b")[0].GetString()!, CultureInfo.InvariantCulture),
                Ask = decimal.Parse(data.GetProperty("a")[0].GetString()!, CultureInfo.InvariantCulture),
                Volume24h = decimal.Parse(data.GetProperty("v")[1].GetString()!, CultureInfo.InvariantCulture) * price,
                Change24h = Math.Round(change, 8),
                Change24hPercent = open > 0 ? Math.Round(change / open * 100, 4) : 0,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch Kraken price for {Symbol}", symbol);
            return null;
        }
    }
}
