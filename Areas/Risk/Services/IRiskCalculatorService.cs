using NavegaStudio.Areas.Risk.Models;

namespace NavegaStudio.Areas.Risk.Services;

public interface IRiskCalculatorService
{
    ServiceResult<RiskCalculationResult> Calculate(RiskInput input);
}
