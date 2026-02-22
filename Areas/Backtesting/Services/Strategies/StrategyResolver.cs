using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public static class StrategyResolver
{
    public static IStrategy Resolve(StrategyType strategyType) => strategyType switch
    {
        StrategyType.SmaCrossover => new SmaCrossoverStrategy(),
        StrategyType.Rsi => new RsiStrategy(),
        StrategyType.Macd => new MacdStrategy(),
        StrategyType.BollingerBands => new BollingerBandsStrategy(),
        StrategyType.EmaCrossover => new EmaCrossoverStrategy(),
        StrategyType.StochasticOscillator => new StochasticOscillatorStrategy(),
        StrategyType.Adx => new AdxStrategy(),
        StrategyType.DonchianBreakout => new DonchianBreakoutStrategy(),
        StrategyType.VwapCross => new VwapCrossStrategy(),
        StrategyType.MeanRevisionZScore => new MeanReversionZScoreStrategy(),
        _ => throw new ArgumentOutOfRangeException(nameof(strategyType), $"Unknown strategy: {strategyType}")
    };
}
