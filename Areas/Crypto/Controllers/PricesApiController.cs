using NavegaStudio.Areas.Crypto.Services;
using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Areas.Crypto.Controllers;

[ApiController]
[Route("api/prices")]
public class PricesApiController : ControllerBase
{
    private readonly ExchangeService _exchangeService;

    public PricesApiController(ExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prices = await _exchangeService.GetAllPricesAsync();
        return Ok(prices);
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> Get(string symbol)
    {
        var snapshot = await _exchangeService.GetPriceSnapshotAsync(symbol.ToUpper());
        if (snapshot.Exchanges.Count == 0)
            return NotFound(new { message = $"No price data found for {symbol}" });
        return Ok(snapshot);
    }

    [HttpGet("symbols")]
    public IActionResult GetSymbols()
    {
        return Ok(_exchangeService.GetSupportedSymbols());
    }
}
