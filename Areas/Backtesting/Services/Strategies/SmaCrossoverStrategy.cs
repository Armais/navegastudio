using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class SmaCrossoverStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int shortP = p.ShortSmaPeriod ?? 10;
        int longP = p.LongSmaPeriod ?? 50;
        return $"SMA Crossover ({shortP}/{longP})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => p.LongSmaPeriod ?? 50;

    public string? ValidateParameters(StrategyParameters p)
    {
        int shortP = p.ShortSmaPeriod ?? 10;
        int longP = p.LongSmaPeriod ?? 50;
        if (shortP < 2 || shortP > 50) return "Short SMA period must be between 2 and 50.";
        if (longP < 10 || longP > 200) return "Long SMA period must be between 10 and 200.";
        if (shortP >= longP) return "Short SMA period must be less than Long SMA period.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int shortP = p.ShortSmaPeriod ?? 10;
        int longP = p.LongSmaPeriod ?? 50;
        var signals = new List<Signal>();

        for (int i = longP - 1; i < prices.Count; i++)
        {
            decimal shortSma = IndicatorHelper.CalculateSma(prices, i, shortP);
            decimal longSma = IndicatorHelper.CalculateSma(prices, i, longP);
            decimal prevShortSma = i > longP - 1 ? IndicatorHelper.CalculateSma(prices, i - 1, shortP) : longSma;
            decimal prevLongSma = i > longP - 1 ? IndicatorHelper.CalculateSma(prices, i - 1, longP) : longSma;

            var type = SignalType.Hold;
            if (prevShortSma <= prevLongSma && shortSma > longSma) type = SignalType.Buy;
            else if (prevShortSma >= prevLongSma && shortSma < longSma) type = SignalType.Sell;

            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
