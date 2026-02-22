using NavegaStudio.Areas.Backtesting.Models;

namespace NavegaStudio.Areas.Backtesting.Services.Strategies;

public static class IndicatorHelper
{
    public static decimal CalculateSma(List<HistoricalPrice> prices, int endIndex, int period)
    {
        decimal sum = 0;
        for (int i = endIndex - period + 1; i <= endIndex; i++)
            sum += prices[i].Close;
        return sum / period;
    }

    public static decimal[] CalculateEmaArray(List<HistoricalPrice> prices, int period)
    {
        var ema = new decimal[prices.Count];
        if (prices.Count < period) return ema;

        decimal sum = 0;
        for (int i = 0; i < period; i++)
            sum += prices[i].Close;
        ema[period - 1] = sum / period;

        decimal multiplier = 2m / (period + 1);
        for (int i = period; i < prices.Count; i++)
            ema[i] = (prices[i].Close - ema[i - 1]) * multiplier + ema[i - 1];

        return ema;
    }

    public static decimal[] CalculateRsiArray(List<HistoricalPrice> prices, int period)
    {
        var rsi = new decimal[prices.Count];
        if (prices.Count <= period) return rsi;

        decimal gainSum = 0, lossSum = 0;
        for (int i = 1; i <= period; i++)
        {
            decimal change = prices[i].Close - prices[i - 1].Close;
            if (change > 0) gainSum += change;
            else lossSum += Math.Abs(change);
        }

        decimal avgGain = gainSum / period;
        decimal avgLoss = lossSum / period;
        rsi[period] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));

        for (int i = period + 1; i < prices.Count; i++)
        {
            decimal change = prices[i].Close - prices[i - 1].Close;
            decimal gain = change > 0 ? change : 0;
            decimal loss = change < 0 ? Math.Abs(change) : 0;

            avgGain = (avgGain * (period - 1) + gain) / period;
            avgLoss = (avgLoss * (period - 1) + loss) / period;

            rsi[i] = avgLoss == 0 ? 100 : 100 - (100 / (1 + avgGain / avgLoss));
        }

        return rsi;
    }

    public static (decimal[] K, decimal[] D) CalculateStochastic(List<HistoricalPrice> prices, int kPeriod, int dPeriod)
    {
        var k = new decimal[prices.Count];
        var d = new decimal[prices.Count];
        if (prices.Count < kPeriod) return (k, d);

        for (int i = kPeriod - 1; i < prices.Count; i++)
        {
            decimal highest = decimal.MinValue;
            decimal lowest = decimal.MaxValue;
            for (int j = i - kPeriod + 1; j <= i; j++)
            {
                if (prices[j].High > highest) highest = prices[j].High;
                if (prices[j].Low < lowest) lowest = prices[j].Low;
            }
            k[i] = highest == lowest ? 50 : (prices[i].Close - lowest) / (highest - lowest) * 100;
        }

        for (int i = kPeriod - 1 + dPeriod - 1; i < prices.Count; i++)
        {
            decimal sum = 0;
            for (int j = i - dPeriod + 1; j <= i; j++)
                sum += k[j];
            d[i] = sum / dPeriod;
        }

        return (k, d);
    }

    public static (decimal[] Adx, decimal[] PlusDI, decimal[] MinusDI) CalculateAdx(List<HistoricalPrice> prices, int period)
    {
        var adx = new decimal[prices.Count];
        var plusDI = new decimal[prices.Count];
        var minusDI = new decimal[prices.Count];
        if (prices.Count < period + 1) return (adx, plusDI, minusDI);

        var tr = new decimal[prices.Count];
        var plusDM = new decimal[prices.Count];
        var minusDM = new decimal[prices.Count];

        for (int i = 1; i < prices.Count; i++)
        {
            decimal highDiff = prices[i].High - prices[i - 1].High;
            decimal lowDiff = prices[i - 1].Low - prices[i].Low;

            plusDM[i] = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;
            minusDM[i] = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;

            decimal hl = prices[i].High - prices[i].Low;
            decimal hc = Math.Abs(prices[i].High - prices[i - 1].Close);
            decimal lc = Math.Abs(prices[i].Low - prices[i - 1].Close);
            tr[i] = Math.Max(hl, Math.Max(hc, lc));
        }

        decimal smoothTR = 0, smoothPlusDM = 0, smoothMinusDM = 0;
        for (int i = 1; i <= period; i++)
        {
            smoothTR += tr[i];
            smoothPlusDM += plusDM[i];
            smoothMinusDM += minusDM[i];
        }

        plusDI[period] = smoothTR == 0 ? 0 : smoothPlusDM / smoothTR * 100;
        minusDI[period] = smoothTR == 0 ? 0 : smoothMinusDM / smoothTR * 100;

        var dx = new decimal[prices.Count];
        decimal diSum = plusDI[period] + minusDI[period];
        dx[period] = diSum == 0 ? 0 : Math.Abs(plusDI[period] - minusDI[period]) / diSum * 100;

        for (int i = period + 1; i < prices.Count; i++)
        {
            smoothTR = smoothTR - smoothTR / period + tr[i];
            smoothPlusDM = smoothPlusDM - smoothPlusDM / period + plusDM[i];
            smoothMinusDM = smoothMinusDM - smoothMinusDM / period + minusDM[i];

            plusDI[i] = smoothTR == 0 ? 0 : smoothPlusDM / smoothTR * 100;
            minusDI[i] = smoothTR == 0 ? 0 : smoothMinusDM / smoothTR * 100;

            diSum = plusDI[i] + minusDI[i];
            dx[i] = diSum == 0 ? 0 : Math.Abs(plusDI[i] - minusDI[i]) / diSum * 100;
        }

        if (prices.Count >= 2 * period)
        {
            decimal dxSum = 0;
            for (int i = period; i < 2 * period; i++)
                dxSum += dx[i];
            adx[2 * period - 1] = dxSum / period;

            for (int i = 2 * period; i < prices.Count; i++)
                adx[i] = (adx[i - 1] * (period - 1) + dx[i]) / period;
        }

        return (adx, plusDI, minusDI);
    }

    public static (decimal[] Upper, decimal[] Lower) CalculateDonchian(List<HistoricalPrice> prices, int entryPeriod, int exitPeriod)
    {
        var upper = new decimal[prices.Count];
        var lower = new decimal[prices.Count];
        int maxPeriod = Math.Max(entryPeriod, exitPeriod);
        if (prices.Count < maxPeriod) return (upper, lower);

        for (int i = entryPeriod - 1; i < prices.Count; i++)
        {
            decimal highest = decimal.MinValue;
            for (int j = i - entryPeriod + 1; j <= i; j++)
                if (prices[j].High > highest) highest = prices[j].High;
            upper[i] = highest;
        }

        for (int i = exitPeriod - 1; i < prices.Count; i++)
        {
            decimal lowest = decimal.MaxValue;
            for (int j = i - exitPeriod + 1; j <= i; j++)
                if (prices[j].Low < lowest) lowest = prices[j].Low;
            lower[i] = lowest;
        }

        return (upper, lower);
    }

    public static decimal[] CalculateVwap(List<HistoricalPrice> prices, int period)
    {
        var vwap = new decimal[prices.Count];
        if (prices.Count < period) return vwap;

        for (int i = period - 1; i < prices.Count; i++)
        {
            decimal sumPV = 0;
            long sumV = 0;
            for (int j = i - period + 1; j <= i; j++)
            {
                decimal typical = (prices[j].High + prices[j].Low + prices[j].Close) / 3;
                sumPV += typical * prices[j].Volume;
                sumV += prices[j].Volume;
            }
            vwap[i] = sumV == 0 ? prices[i].Close : sumPV / sumV;
        }

        return vwap;
    }

    public static decimal[] CalculateZScore(List<HistoricalPrice> prices, int period)
    {
        var zScore = new decimal[prices.Count];
        if (prices.Count < period) return zScore;

        for (int i = period - 1; i < prices.Count; i++)
        {
            decimal sum = 0;
            for (int j = i - period + 1; j <= i; j++)
                sum += prices[j].Close;
            decimal mean = sum / period;

            decimal sumSq = 0;
            for (int j = i - period + 1; j <= i; j++)
            {
                decimal diff = prices[j].Close - mean;
                sumSq += diff * diff;
            }
            decimal stdDev = (decimal)Math.Sqrt((double)(sumSq / period));

            zScore[i] = stdDev == 0 ? 0 : (prices[i].Close - mean) / stdDev;
        }

        return zScore;
    }
}
