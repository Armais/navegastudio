using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public class MeanReversionZScoreStrategy : IStrategy
{
    public string GetDescription(StrategyParameters p)
    {
        int period = p.ZScorePeriod ?? 20;
        double entry = p.ZScoreEntry ?? 2.0;
        double exit = p.ZScoreExit ?? 0.0;
        return $"Z-Score Mean Reversion ({period}, entry={entry}, exit={exit})";
    }

    public int GetRequiredDataPoints(StrategyParameters p) => (p.ZScorePeriod ?? 20) + 1;

    public string? ValidateParameters(StrategyParameters p)
    {
        int period = p.ZScorePeriod ?? 20;
        double entry = p.ZScoreEntry ?? 2.0;
        double exit = p.ZScoreExit ?? 0.0;
        if (period < 5 || period > 200) return "Z-Score period must be between 5 and 200.";
        if (entry < 0.5 || entry > 5.0) return "Z-Score entry threshold must be between 0.5 and 5.0.";
        if (exit < 0.0 || exit > 4.0) return "Z-Score exit threshold must be between 0.0 and 4.0.";
        if (exit >= entry) return "Exit threshold must be less than entry threshold.";
        return null;
    }

    public List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters p)
    {
        int period = p.ZScorePeriod ?? 20;
        decimal entry = (decimal)(p.ZScoreEntry ?? 2.0);
        decimal exit = (decimal)(p.ZScoreExit ?? 0.0);
        var zScore = IndicatorHelper.CalculateZScore(prices, period);
        var signals = new List<Signal>();

        for (int i = period; i < prices.Count; i++)
        {
            var type = SignalType.Hold;
            decimal prevZ = zScore[i - 1];
            decimal currZ = zScore[i];
            if (prevZ >= -entry && currZ < -entry) type = SignalType.Buy;
            else if ((prevZ <= entry && currZ > entry) || (prevZ < exit && currZ >= exit && prevZ < 0)) type = SignalType.Sell;
            signals.Add(new Signal { Index = i, Type = type });
        }
        return signals;
    }
}
