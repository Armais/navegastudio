using NavegaStudio.Areas.Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Areas.Crypto.Controllers;

[ApiController]
[Route("api/arbitrage")]
public class ArbitrageApiController : ControllerBase
{
    private readonly ExchangeService _exchangeService;

    public ArbitrageApiController(ExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOpportunities([FromQuery] decimal minSpread = 0.1m)
    {
        var opportunities = await _exchangeService.DetectArbitrageAsync(minSpread);
        return Ok(opportunities);
    }
}
