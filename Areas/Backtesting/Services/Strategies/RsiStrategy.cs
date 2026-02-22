using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class RsiStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int period = p.RsiPeriod ?? 14;
        int oversold = p.RsiOversold ?? 30;
        int overbought = p.RsiOverbought ?? 70;
        return $"RSI ({period}, {oversold}/{overbought})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => (p.RsiPeriod ?? 14) + 1;

    public string? ValidateParameters(StrategyParameters p)
    {
        int period = p.RsiPeriod ?? 14;
        int oversold = p.RsiOversold ?? 30;
        int overbought = p.RsiOverbought ?? 70;
        if (period < 2 || period > 100) return "RSI period must be between 2 and 100.";
        if (oversold < 1 || oversold > 49) return "RSI oversold level must be between 1 and 49.";
        if (overbought < 51 || overbought > 99) return "RSI overbought level must be between 51 and 99.";
        if (oversold >= overbought) return "RSI oversold must be less than overbought.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int period = p.RsiPeriod ?? 14;
        int oversold = p.RsiOversold ?? 30;
        int overbought = p.RsiOverbought ?? 70;
        var rsi = IndicatorHelper.CalculateRsiArray(prices, period);
        var signals = new List<Signal>();

        for (int i = period + 1; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (rsi[i - 1] <= oversold && rsi[i] > oversold) type = SignalType.Buy;
            else if (rsi[i - 1] >= overbought && rsi[i] < overbought) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
