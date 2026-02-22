using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Backtesting.Models;

public enum StrategyType
{
    [Display(Name = "SMA Crossover")]
    SmaCrossover = 0,

    [Display(Name = "RSI (Relative Strength Index)")]
    Rsi = 1,

    [Display(Name = "MACD")]
    Macd = 2,

    [Display(Name = "Bollinger Bands")]
    BollingerBands = 3,

    [Display(Name = "EMA Crossover")]
    EmaCrossover = 4,

    [Display(Name = "Stochastic Oscillator")]
    StochasticOscillator = 5,

    [Display(Name = "ADX (Average Directional Index)")]
    Adx = 6,

    [Display(Name = "Donchian Breakout")]
    DonchianBreakout = 7,

    [Display(Name = "VWAP Cross")]
    VwapCross = 8,

    [Display(Name = "Mean Reversion (Z-Score)")]
    MeanRevisionZScore = 9
}
