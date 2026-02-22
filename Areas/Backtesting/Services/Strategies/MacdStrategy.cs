using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class MacdStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int fast = p.MacdFastPeriod ?? 12;
        int slow = p.MacdSlowPeriod ?? 26;
        int signal = p.MacdSignalPeriod ?? 9;
        return $"MACD ({fast}/{slow}/{signal})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) =>
        (p.MacdSlowPeriod ?? 26) + (p.MacdSignalPeriod ?? 9);

    public string? ValidateParameters(StrategyParameters p)
    {
        int fast = p.MacdFastPeriod ?? 12;
        int slow = p.MacdSlowPeriod ?? 26;
        int signal = p.MacdSignalPeriod ?? 9;
        if (fast < 2 || fast > 50) return "MACD fast period must be between 2 and 50.";
        if (slow < 10 || slow > 200) return "MACD slow period must be between 10 and 200.";
        if (signal < 2 || signal > 50) return "MACD signal period must be between 2 and 50.";
        if (fast >= slow) return "MACD fast period must be less than slow period.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int fast = p.MacdFastPeriod ?? 12;
        int slow = p.MacdSlowPeriod ?? 26;
        int signal = p.MacdSignalPeriod ?? 9;

        var fastEma = IndicatorHelper.CalculateEmaArray(prices, fast);
        var slowEma = IndicatorHelper.CalculateEmaArray(prices, slow);

        var macdLine = new decimal[prices.Count];
        int macdStart = slow - 1;
        for (int i = macdStart; i < prices.Count; i++)
            macdLine[i] = fastEma[i] - slowEma[i];

        var signalLine = new decimal[prices.Count];
        if (prices.Count > macdStart + signal)
        {
            decimal sum = 0;
            for (int i = macdStart; i < macdStart + signal; i++)
                sum += macdLine[i];
            signalLine[macdStart + signal - 1] = sum / signal;

            decimal multiplier = 2m / (signal + 1);
            for (int i = macdStart + signal; i < prices.Count; i++)
                signalLine[i] = (macdLine[i] - signalLine[i - 1]) * multiplier + signalLine[i - 1];
        }

        var signals = new List<Signal>();
        int startIndex = macdStart + signal;

        for (int i = startIndex; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (macdLine[i - 1] <= signalLine[i - 1] && macdLine[i] > signalLine[i]) type = SignalType.Buy;
            else if (macdLine[i - 1] >= signalLine[i - 1] && macdLine[i] < signalLine[i]) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
