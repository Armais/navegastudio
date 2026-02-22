using NavegaStudio.Data;
using NavegaStudio.Areas.Backtesting.Models;
using NavegaStudio.Areas.Backtesting.Services.Strategies;
using Microsoft.EntityFrameworkCore;

namespace NavegaStudio.Areas.Backtesting.Services;

public class BacktestService
{
    private readonly BacktestDbContext _context;

    public BacktestService(BacktestDbContext context)
    {
        _context = context;
    }

    public async Task<BacktestResult> RunAsync(BacktestRequest request)
    {
        var strategy = StrategyResolver.Resolve(request.StrategyType);

        var validationError = strategy.ValidateParameters(request.Parameters);
        if (validationError != null)
            throw new InvalidOperationException(validationError);

        int requiredPoints = strategy.GetRequiredDataPoints(request.Parameters);

        var prices = await _context.HistoricalPrices
            .Where(p => p.Symbol == request.Symbol
                        && p.Date >= request.StartDate
                        && p.Date <= request.EndDate)
            .OrderBy(p => p.Date)
            .ToListAsync();

        if (prices.Count < requiredPoints)
            throw new InvalidOperationException(
                $"Not enough data. Need at least {requiredPoints} data points, got {prices.Count}.");

        var signals = strategy.GenerateSignals(prices, request.Parameters);
        var description = strategy.GetDescription(request.Parameters);

        return await ExecuteTrades(prices, signals, request, description);
    }

    private async Task<BacktestResult> ExecuteTrades(
        List<HistoricalPrice> prices,
        List<Signal> signals,
        BacktestRequest request,
        string strategyDescription)
    {
        decimal capital = request.InitialCapital;
        decimal peakCapital = capital;
        decimal maxDrawdown = 0;
        decimal shares = 0;
        bool inPosition = false;
        DateTime entryDate = default;
        decimal entryPrice = 0;
        decimal stopLossPrice = 0;
        decimal takeProfitPrice = 0;
        decimal totalCommissions = 0;

        var trades = new List<Trade>();
        var equityCurve = new List<EquityPoint>();
        var drawdownCurve = new List<DrawdownPoint>();
        var benchmarkCurve = new List<EquityPoint>();
        var dailyReturns = new List<decimal>();
        decimal previousEquity = capital;

        // Build a lookup: index -> signal type for O(1) access
        var signalMap = new Dictionary<int, SignalType>();
        foreach (var s in signals)
            signalMap[s.Index] = s.Type;

        // Benchmark buy-and-hold: buy at first close, hold until last close
        decimal benchShares = Math.Floor(request.InitialCapital / prices[0].Close);
        decimal benchCash = request.InitialCapital - benchShares * prices[0].Close;

        for (int i = 0; i < prices.Count; i++)
        {
            var bar = prices[i];
            string? exitReason = null;
            decimal exitPrice = 0;

            // --- Check SL/TP on open positions ---
            if (inPosition)
            {
                // Stop-loss: triggered if low <= SL price (SL has priority)
                if (request.StopLossPct.HasValue && bar.Low <= stopLossPrice)
                {
                    exitPrice = stopLossPrice;
                    exitReason = "StopLoss";
                }
                // Take-profit: triggered if high >= TP price
                else if (request.TakeProfitPct.HasValue && bar.High >= takeProfitPrice)
                {
                    exitPrice = takeProfitPrice;
                    exitReason = "TakeProfit";
                }
                // Signal sell
                else if (signalMap.TryGetValue(i, out var sig) && sig == SignalType.Sell)
                {
                    exitPrice = ApplySlippage(bar.Close, request.SlippagePct, isBuy: false);
                    exitReason = "Signal";
                }

                if (exitReason != null)
                {
                    decimal commission = request.CommissionPerTrade;
                    totalCommissions += commission;
                    decimal proceeds = shares * exitPrice - commission;
                    decimal pnl = proceeds - (shares * entryPrice);
                    capital += proceeds;

                    trades.Add(new Trade
                    {
                        EntryDate = entryDate,
                        ExitDate = bar.Date,
                        EntryPrice = Math.Round(entryPrice, 4),
                        ExitPrice = Math.Round(exitPrice, 4),
                        Quantity = shares,
                        ProfitLoss = Math.Round(pnl, 2),
                        ProfitLossPercent = entryPrice > 0
                            ? Math.Round((exitPrice - entryPrice) / entryPrice * 100, 4)
                            : 0,
                        Type = "Long",
                        ExitReason = exitReason
                    });

                    shares = 0;
                    inPosition = false;
                }
            }

            // --- Entry logic ---
            if (!inPosition && signalMap.TryGetValue(i, out var buySig) && buySig == SignalType.Buy)
            {
                decimal rawPrice = ApplySlippage(bar.Close, request.SlippagePct, isBuy: true);
                decimal commission = request.CommissionPerTrade;

                // Position sizing
                decimal positionShares;
                if (request.RiskPerTrade >= 100m)
                {
                    // All-in: use all available capital (backward compatible)
                    positionShares = Math.Floor((capital - commission) / rawPrice);
                }
                else if (request.StopLossPct.HasValue && request.StopLossPct.Value > 0)
                {
                    // Risk-based sizing with stop-loss distance
                    decimal riskAmount = capital * (request.RiskPerTrade / 100m);
                    decimal slDistance = rawPrice * (request.StopLossPct.Value / 100m);
                    positionShares = slDistance > 0 ? Math.Floor(riskAmount / slDistance) : 0;
                    // Cap to affordable shares
                    decimal maxAffordable = Math.Floor((capital - commission) / rawPrice);
                    positionShares = Math.Min(positionShares, maxAffordable);
                }
                else
                {
                    // Risk % of capital without stop-loss
                    decimal riskCapital = capital * (request.RiskPerTrade / 100m);
                    positionShares = Math.Floor(riskCapital / rawPrice);
                }

                if (positionShares > 0 && capital >= positionShares * rawPrice + commission)
                {
                    totalCommissions += commission;
                    capital -= positionShares * rawPrice + commission;
                    shares = positionShares;
                    entryPrice = rawPrice;
                    entryDate = bar.Date;
                    inPosition = true;

                    // Set SL/TP levels
                    stopLossPrice = request.StopLossPct.HasValue
                        ? Math.Round(entryPrice * (1m - request.StopLossPct.Value / 100m), 4)
                        : 0;
                    takeProfitPrice = request.TakeProfitPct.HasValue
                        ? Math.Round(entryPrice * (1m + request.TakeProfitPct.Value / 100m), 4)
                        : decimal.MaxValue;
                }
            }

            // --- Record equity, drawdown, daily return for EVERY bar ---
            decimal equity = capital + shares * bar.Close;
            equityCurve.Add(new EquityPoint { Date = bar.Date, Equity = Math.Round(equity, 2) });

            if (equity > peakCapital) peakCapital = equity;
            decimal drawdown = peakCapital > 0 ? (peakCapital - equity) / peakCapital * 100 : 0;
            if (drawdown > maxDrawdown) maxDrawdown = drawdown;
            drawdownCurve.Add(new DrawdownPoint { Date = bar.Date, DrawdownPct = Math.Round(-drawdown, 4) });

            decimal dailyReturn = previousEquity > 0 ? (equity - previousEquity) / previousEquity : 0;
            dailyReturns.Add(dailyReturn);
            previousEquity = equity;

            // Benchmark equity
            decimal benchEquity = benchCash + benchShares * bar.Close;
            benchmarkCurve.Add(new EquityPoint { Date = bar.Date, Equity = Math.Round(benchEquity, 2) });
        }

        // --- Force close open position at end ---
        if (inPosition)
        {
            var lastBar = prices[^1];
            decimal lastPrice = lastBar.Close;
            decimal commission = request.CommissionPerTrade;
            totalCommissions += commission;
            decimal proceeds = shares * lastPrice - commission;
            decimal pnl = proceeds - (shares * entryPrice);
            capital += proceeds;

            trades.Add(new Trade
            {
                EntryDate = entryDate,
                ExitDate = lastBar.Date,
                EntryPrice = Math.Round(entryPrice, 4),
                ExitPrice = Math.Round(lastPrice, 4),
                Quantity = shares,
                ProfitLoss = Math.Round(pnl, 2),
                ProfitLossPercent = entryPrice > 0
                    ? Math.Round((lastPrice - entryPrice) / entryPrice * 100, 4)
                    : 0,
                Type = "Long",
                ExitReason = "EndOfData"
            });

            shares = 0;
        }

        // --- Compute metrics ---
        var winTrades = trades.Where(t => t.ProfitLoss > 0).ToList();
        var loseTrades = trades.Where(t => t.ProfitLoss <= 0).ToList();
        decimal avgWin = winTrades.Count > 0 ? winTrades.Average(t => t.ProfitLoss) : 0;
        decimal avgLoss = loseTrades.Count > 0 ? Math.Abs(loseTrades.Average(t => t.ProfitLoss)) : 0;
        decimal grossProfit = winTrades.Sum(t => t.ProfitLoss);
        decimal grossLoss = Math.Abs(loseTrades.Sum(t => t.ProfitLoss));
        decimal profitFactor = grossLoss > 0 ? grossProfit / grossLoss : grossProfit > 0 ? 999.99m : 0;
        decimal winRate = trades.Count > 0 ? (decimal)winTrades.Count / trades.Count * 100 : 0;
        decimal loseRate = trades.Count > 0 ? (decimal)loseTrades.Count / trades.Count : 0;
        decimal winRateRatio = trades.Count > 0 ? (decimal)winTrades.Count / trades.Count : 0;
        decimal expectancy = avgWin * winRateRatio - avgLoss * loseRate;

        decimal benchFinal = benchCash + benchShares * prices[^1].Close;
        decimal benchReturn = request.InitialCapital > 0
            ? (benchFinal - request.InitialCapital) / request.InitialCapital * 100
            : 0;

        var result = new BacktestResult
        {
            Symbol = request.Symbol,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            InitialCapital = request.InitialCapital,
            StrategyType = request.StrategyType,
            StrategyDescription = strategyDescription,
            FinalCapital = Math.Round(capital, 2),
            TotalReturn = request.InitialCapital > 0
                ? Math.Round((capital - request.InitialCapital) / request.InitialCapital * 100, 4)
                : 0,
            MaxDrawdown = Math.Round(maxDrawdown, 4),
            TotalTrades = trades.Count,
            WinningTrades = winTrades.Count,
            LosingTrades = loseTrades.Count,
            WinRate = Math.Round(winRate, 4),
            SharpeRatio = CalculateSharpe(dailyReturns),
            SortinoRatio = CalculateSortino(dailyReturns),
            ProfitFactor = Math.Round(Math.Min(profitFactor, 999.99m), 4),
            Expectancy = Math.Round(expectancy, 4),
            AvgWin = Math.Round(avgWin, 2),
            AvgLoss = Math.Round(avgLoss, 2),
            MaxConsecutiveWins = CalculateMaxConsecutive(trades, isWins: true),
            MaxConsecutiveLosses = CalculateMaxConsecutive(trades, isWins: false),
            TotalCommissions = Math.Round(totalCommissions, 2),
            BenchmarkReturn = Math.Round(benchReturn, 4),
            BenchmarkFinalCapital = Math.Round(benchFinal, 2),
            Trades = trades,
            EquityCurve = equityCurve,
            DrawdownCurve = drawdownCurve,
            BenchmarkCurve = benchmarkCurve
        };

        _context.BacktestResults.Add(result);
        await _context.SaveChangesAsync();

        return result;
    }

    private static decimal ApplySlippage(decimal price, decimal slippagePct, bool isBuy)
    {
        if (slippagePct == 0) return price;
        decimal factor = slippagePct / 100m;
        return isBuy ? price * (1m + factor) : price * (1m - factor);
    }

    private static decimal CalculateSharpe(List<decimal> dailyReturns)
    {
        if (dailyReturns.Count < 2) return 0;

        decimal avg = dailyReturns.Average();
        decimal sumSquaredDiff = dailyReturns.Sum(r => (r - avg) * (r - avg));
        decimal stdDev = (decimal)Math.Sqrt((double)(sumSquaredDiff / (dailyReturns.Count - 1)));

        if (stdDev == 0) return 0;

        decimal annualizedReturn = avg * 252;
        decimal annualizedStdDev = stdDev * (decimal)Math.Sqrt(252);

        return Math.Round(annualizedReturn / annualizedStdDev, 4);
    }

    private static decimal CalculateSortino(List<decimal> dailyReturns)
    {
        if (dailyReturns.Count < 2) return 0;

        decimal avg = dailyReturns.Average();
        var downsideReturns = dailyReturns.Where(r => r < 0).ToList();
        if (downsideReturns.Count == 0) return avg > 0 ? 99.99m : 0;

        decimal sumSquaredDownside = downsideReturns.Sum(r => r * r);
        decimal downsideDev = (decimal)Math.Sqrt((double)(sumSquaredDownside / dailyReturns.Count));

        if (downsideDev == 0) return 0;

        decimal annualizedReturn = avg * 252;
        decimal annualizedDownsideDev = downsideDev * (decimal)Math.Sqrt(252);

        return Math.Round(annualizedReturn / annualizedDownsideDev, 4);
    }

    private static int CalculateMaxConsecutive(List<Trade> trades, bool isWins)
    {
        int max = 0;
        int current = 0;
        foreach (var t in trades)
        {
            bool match = isWins ? t.ProfitLoss > 0 : t.ProfitLoss <= 0;
            if (match)
            {
                current++;
                if (current > max) max = current;
            }
            else
            {
                current = 0;
            }
        }
        return max;
    }
}
