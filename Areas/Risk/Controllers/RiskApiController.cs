using Microsoft.AspNetCore.Mvc;
using NavegaStudio.Areas.Risk.Models;
using NavegaStudio.Areas.Risk.Services;

namespace NavegaStudio.Areas.Risk.Controllers;

[ApiController]
[Route("api/risk")]
public class RiskApiController : ControllerBase
{
    private readonly IRiskCalculatorService _riskService;

    public RiskApiController(IRiskCalculatorService riskService)
    {
        _riskService = riskService;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] RiskInput input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = _riskService.Calculate(input);

        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }
}
