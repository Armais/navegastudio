namespace NavegaStudio.Areas.Backtesting.Models;

public class StrategyParameters
{
    public int? ShortSmaPeriod { get; set; }
    public int? LongSmaPeriod { get; set; }

    public int? RsiPeriod { get; set; }
    public int? RsiOversold { get; set; }
    public int? RsiOverbought { get; set; }

    public int? MacdFastPeriod { get; set; }
    public int? MacdSlowPeriod { get; set; }
    public int? MacdSignalPeriod { get; set; }

    public int? BollingerPeriod { get; set; }
    public double? BollingerMultiplier { get; set; }

    public int? ShortEmaPeriod { get; set; }
    public int? LongEmaPeriod { get; set; }

    public int? StochasticKPeriod { get; set; }
    public int? StochasticDPeriod { get; set; }
    public int? StochasticOverbought { get; set; }
    public int? StochasticOversold { get; set; }

    public int? AdxPeriod { get; set; }
    public int? AdxThreshold { get; set; }

    public int? DonchianEntryPeriod { get; set; }
    public int? DonchianExitPeriod { get; set; }

    public int? VwapPeriod { get; set; }

    public int? ZScorePeriod { get; set; }
    public double? ZScoreEntry { get; set; }
    public double? ZScoreExit { get; set; }
}
