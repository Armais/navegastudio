using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class StochasticOscillatorStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int k = p.StochasticKPeriod ?? 14;
        int d = p.StochasticDPeriod ?? 3;
        int ob = p.StochasticOverbought ?? 80;
        int os = p.StochasticOversold ?? 20;
        return $"Stochastic (%K={k}, %D={d}, {os}/{ob})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) =>
        (p.StochasticKPeriod ?? 14) + (p.StochasticDPeriod ?? 3);

    public string? ValidateParameters(StrategyParameters p)
    {
        int k = p.StochasticKPeriod ?? 14;
        int d = p.StochasticDPeriod ?? 3;
        int ob = p.StochasticOverbought ?? 80;
        int os = p.StochasticOversold ?? 20;
        if (k < 2 || k > 100) return "Stochastic %K period must be between 2 and 100.";
        if (d < 1 || d > 50) return "Stochastic %D period must be between 1 and 50.";
        if (os < 1 || os > 49) return "Oversold level must be between 1 and 49.";
        if (ob < 51 || ob > 99) return "Overbought level must be between 51 and 99.";
        if (os >= ob) return "Oversold must be less than overbought.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int kPeriod = p.StochasticKPeriod ?? 14;
        int dPeriod = p.StochasticDPeriod ?? 3;
        int ob = p.StochasticOverbought ?? 80;
        int os = p.StochasticOversold ?? 20;
        var (k, d) = IndicatorHelper.CalculateStochastic(prices, kPeriod, dPeriod);
        var signals = new List<Signal>();
        int start = kPeriod - 1 + dPeriod - 1;

        for (int i = start + 1; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            if (k[i - 1] <= d[i - 1] && k[i] > d[i] && d[i] < os) type = SignalType.Buy;
            else if (k[i - 1] >= d[i - 1] && k[i] < d[i] && d[i] > ob) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
