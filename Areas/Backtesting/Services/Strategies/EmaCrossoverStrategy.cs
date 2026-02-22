using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class EmaCrossoverStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int shortP = p.ShortEmaPeriod ?? 12;
        int longP = p.LongEmaPeriod ?? 26;
        return $"EMA Crossover ({shortP}/{longP})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => p.LongEmaPeriod ?? 26;

    public string? ValidateParameters(StrategyParameters p)
    {
        int shortP = p.ShortEmaPeriod ?? 12;
        int longP = p.LongEmaPeriod ?? 26;
        if (shortP < 2 || shortP > 50) return "Short EMA period must be between 2 and 50.";
        if (longP < 10 || longP > 200) return "Long EMA period must be between 10 and 200.";
        if (shortP >= longP) return "Short EMA period must be less than Long EMA period.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int shortP = p.ShortEmaPeriod ?? 12;
        int longP = p.LongEmaPeriod ?? 26;
        var shortEma = IndicatorHelper.CalculateEmaArray(prices, shortP);
        var longEma = IndicatorHelper.CalculateEmaArray(prices, longP);
        var signals = new List<Signal>();

        for (int i = longP; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (shortEma[i - 1] <= longEma[i - 1] && shortEma[i] > longEma[i]) type = SignalType.Buy;
            else if (shortEma[i - 1] >= longEma[i - 1] && shortEma[i] < longEma[i]) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
