using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class AdxStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int period = p.AdxPeriod ?? 14;
        int threshold = p.AdxThreshold ?? 25;
        return $"ADX ({period}, threshold={threshold})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => (p.AdxPeriod ?? 14) * 2 + 1;

    public string? ValidateParameters(StrategyParameters p)
    {
        int period = p.AdxPeriod ?? 14;
        int threshold = p.AdxThreshold ?? 25;
        if (period < 2 || period > 100) return "ADX period must be between 2 and 100.";
        if (threshold < 1 || threshold > 99) return "ADX threshold must be between 1 and 99.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int period = p.AdxPeriod ?? 14;
        int threshold = p.AdxThreshold ?? 25;
        var (adx, plusDI, minusDI) = IndicatorHelper.CalculateAdx(prices, period);
        var signals = new List<Signal>();
        int start = 2 * period;

        for (int i = start; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            bool strongTrend = adx[i] >= threshold;
            if (strongTrend && plusDI[i - 1] <= minusDI[i - 1] && plusDI[i] > minusDI[i]) type = SignalType.Buy;
            else if (strongTrend && plusDI[i - 1] >= minusDI[i - 1] && plusDI[i] < minusDI[i]) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
