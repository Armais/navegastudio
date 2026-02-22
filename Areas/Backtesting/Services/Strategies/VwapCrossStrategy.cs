using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class VwapCrossStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int period = p.VwapPeriod ?? 20;
        return $"VWAP Cross ({period})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => (p.VwapPeriod ?? 20) + 1;

    public string? ValidateParameters(StrategyParameters p)
    {
        int period = p.VwapPeriod ?? 20;
        if (period < 2 || period > 200) return "VWAP period must be between 2 and 200.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int period = p.VwapPeriod ?? 20;
        var vwap = IndicatorHelper.CalculateVwap(prices, period);
        var signals = new List<Signal>();

        for (int i = period; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (prices[i - 1].Close <= vwap[i - 1] && prices[i].Close > vwap[i]) type = SignalType.Buy;
            else if (prices[i - 1].Close >= vwap[i - 1] && prices[i].Close < vwap[i]) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
