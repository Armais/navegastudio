using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class DonchianBreakoutStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int entry = p.DonchianEntryPeriod ?? 20;
        int exit = p.DonchianExitPeriod ?? 10;
        return $"Donchian Breakout (entry={entry}, exit={exit})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) =>
        Math.Max(p.DonchianEntryPeriod ?? 20, p.DonchianExitPeriod ?? 10) + 1;

    public string? ValidateParameters(StrategyParameters p)
    {
        int entry = p.DonchianEntryPeriod ?? 20;
        int exit = p.DonchianExitPeriod ?? 10;
        if (entry < 2 || entry > 200) return "Donchian entry period must be between 2 and 200.";
        if (exit < 2 || exit > 200) return "Donchian exit period must be between 2 and 200.";
        if (exit >= entry) return "Exit period should be less than entry period.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int entryPeriod = p.DonchianEntryPeriod ?? 20;
        int exitPeriod = p.DonchianExitPeriod ?? 10;
        var (upper, lower) = IndicatorHelper.CalculateDonchian(prices, entryPeriod, exitPeriod);
        var signals = new List<Signal>();
        int start = Math.Max(entryPeriod, exitPeriod);

        for (int i = start; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (prices[i].Close > upper[i - 1]) type = SignalType.Buy;
            else if (prices[i].Close < lower[i - 1]) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
