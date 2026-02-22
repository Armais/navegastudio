using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class BollingerBandsStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int period = p.BollingerPeriod ?? 20;
        double mult = p.BollingerMultiplier ?? 2.0;
        return $"Bollinger Bands ({period}, {mult:F1}x)";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => p.BollingerPeriod ?? 20;

    public string? ValidateParameters(StrategyParameters p)
    {
        int period = p.BollingerPeriod ?? 20;
        double mult = p.BollingerMultiplier ?? 2.0;
        if (period < 5 || period > 100) return "Bollinger period must be between 5 and 100.";
        if (mult < 0.5 || mult > 5.0) return "Bollinger multiplier must be between 0.5 and 5.0.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int period = p.BollingerPeriod ?? 20;
        double mult = p.BollingerMultiplier ?? 2.0;
        var signals = new List<Signal>();

        for (int i = period - 1; i < prices.Count; i++)
        {
            decimal sma = IndicatorHelper.CalculateSma(prices, i, period);
            decimal sumSq = 0;
            for (int j = i - period + 1; j <= i; j++)
            {
                decimal diff = prices[j].Close - sma;
                sumSq += diff * diff;
            }
            decimal stdDev = (decimal)Math.Sqrt((double)(sumSq / period));
            decimal upperBand = sma + (decimal)mult * stdDev;
            decimal lowerBand = sma - (decimal)mult * stdDev;
            decimal price = prices[i].Close;

            var type = SignalType.Hold;
            if (price <= lowerBand) type = SignalType.Buy;
            else if (price >= upperBand) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
