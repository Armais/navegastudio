using NavegaStudio.Areas.Risk.Models;

namespace NavegaStudio.Areas.Risk.Services;

public class RiskCalculatorService : IRiskCalculatorService
{
    public ServiceResult<RiskCalculationResult> Calculate(RiskInput input)
    {
        var validationError = Validate(input);
        if (validationError is not null)
            return ServiceResult<RiskCalculationResult>.Error(validationError);

        bool isRiskCalcMode = input.Mode == "RiskCalculator";

        decimal stopLossDistance = Math.Abs(input.EntryPrice - input.StopLoss);
        decimal stopLossPercent = stopLossDistance / input.EntryPrice * 100;

        var (positionSize, riskAmount, riskPercent) = isRiskCalcMode
            ? CalculateRisk(input, stopLossDistance)
            : CalculatePositionSize(input, stopLossDistance);

        decimal positionValue = positionSize * input.EntryPrice;
        decimal maxLoss = positionSize * stopLossDistance;
        decimal commissionCost = positionValue * (input.CommissionPercent / 100) * 2;
        decimal netMaxLoss = maxLoss + commissionCost;
        decimal leverage = input.AccountCapital > 0 ? positionValue / input.AccountCapital : 0;

        var result = new RiskCalculationResult
        {
            Mode = input.Mode,
            RiskPercent = Math.Round(riskPercent, 2),
            PositionSize = positionSize,
            PositionValue = Math.Round(positionValue, 2),
            RiskAmount = Math.Round(riskAmount, 2),
            StopLossDistance = Math.Round(stopLossDistance, 4),
            StopLossPercent = Math.Round(stopLossPercent, 2),
            MaxLoss = Math.Round(maxLoss, 2),
            CommissionCost = Math.Round(commissionCost, 2),
            NetMaxLoss = Math.Round(netMaxLoss, 2),
            Leverage = Math.Round(leverage, 2),
            PortfolioRiskPercent = Math.Round(netMaxLoss / input.AccountCapital * 100, 2),
            TradeDirection = input.TradeDirection
        };

        if (input.TakeProfit.HasValue)
            CalculateTakeProfit(result, input, positionSize, stopLossDistance, commissionCost);

        return ServiceResult<RiskCalculationResult>.Ok(result);
    }

    private static string? Validate(RiskInput input)
    {
        bool isRiskCalcMode = input.Mode == "RiskCalculator";

        if (input.Mode != "RiskCalculator" && input.Mode != "PositionSizer")
            return "Mode must be 'RiskCalculator' or 'PositionSizer'.";

        if (isRiskCalcMode && (!input.PositionSize.HasValue || input.PositionSize.Value < 1))
            return "Position Size is required in Risk Calculator mode.";

        if (!isRiskCalcMode && (!input.RiskPercent.HasValue || input.RiskPercent.Value <= 0))
            return "Risk Percentage is required in Position Sizer mode.";

        bool isLong = input.TradeDirection == "Long";

        if (isLong && input.StopLoss >= input.EntryPrice)
            return "For a Long trade, stop loss must be below entry price.";
        if (!isLong && input.StopLoss <= input.EntryPrice)
            return "For a Short trade, stop loss must be above entry price.";

        if (input.TakeProfit.HasValue)
        {
            if (isLong && input.TakeProfit <= input.EntryPrice)
                return "For a Long trade, take profit must be above entry price.";
            if (!isLong && input.TakeProfit >= input.EntryPrice)
                return "For a Short trade, take profit must be below entry price.";
        }

        return null;
    }

    private static (decimal positionSize, decimal riskAmount, decimal riskPercent) CalculateRisk(
        RiskInput input, decimal stopLossDistance)
    {
        decimal positionSize = input.PositionSize!.Value;
        decimal riskAmount = positionSize * stopLossDistance;
        decimal riskPercent = input.AccountCapital > 0 ? riskAmount / input.AccountCapital * 100 : 0;
        return (positionSize, riskAmount, riskPercent);
    }

    private static (decimal positionSize, decimal riskAmount, decimal riskPercent) CalculatePositionSize(
        RiskInput input, decimal stopLossDistance)
    {
        decimal riskPercent = input.RiskPercent!.Value;
        decimal riskAmount = input.AccountCapital * (riskPercent / 100);
        decimal positionSize = Math.Floor(riskAmount / stopLossDistance);
        if (positionSize < 1) positionSize = 1;
        return (positionSize, riskAmount, riskPercent);
    }

    private static void CalculateTakeProfit(
        RiskCalculationResult result, RiskInput input,
        decimal positionSize, decimal stopLossDistance, decimal commissionCost)
    {
        decimal tpDistance = Math.Abs(input.TakeProfit!.Value - input.EntryPrice);
        decimal tpPercent = tpDistance / input.EntryPrice * 100;
        decimal potentialProfit = positionSize * tpDistance;
        decimal netProfit = potentialProfit - commissionCost;
        decimal rrRatio = stopLossDistance > 0 ? tpDistance / stopLossDistance : 0;

        result.TakeProfitDistance = Math.Round(tpDistance, 4);
        result.TakeProfitPercent = Math.Round(tpPercent, 2);
        result.PotentialProfit = Math.Round(potentialProfit, 2);
        result.NetPotentialProfit = Math.Round(netProfit, 2);
        result.RiskRewardRatio = Math.Round(rrRatio, 2);

        decimal winRate = 0.50m;
        decimal kellyPercent = rrRatio > 0
            ? (winRate * rrRatio - (1 - winRate)) / rrRatio * 100
            : 0;
        result.KellyPercent = Math.Round(Math.Max(kellyPercent, 0), 2);
        result.KellyPositionSize = Math.Round(
            input.AccountCapital * (Math.Max(kellyPercent, 0) / 100) / stopLossDistance, 0);
    }
}
