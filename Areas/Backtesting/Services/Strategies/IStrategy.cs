using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public enum SignalType
{
    Hold,
    Buy,
    Sell
}

public class Signal
{
    public int Index { get; set; }
    public SignalType Type { get; set; }
}

public interface IStrategy
{
    string GetDescription(StrategyParameters parameters);
    int GetRequiredDataPoints(StrategyParameters parameters);
    string? ValidateParameters(StrategyParameters parameters);
    List<Signal> GenerateSignals(List<HistoricalPrice> prices, StrategyParameters parameters);
}
