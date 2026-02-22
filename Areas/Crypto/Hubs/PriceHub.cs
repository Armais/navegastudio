using Microsoft.AspNetCore.SignalR;
using NavegaStudio.Areas.Crypto.Services;

namespace NavegaStudio.Areas.Crypto.Hubs;

public class PriceHub : Hub
{
    private readonly ExchangeService _exchangeService;

    public PriceHub(ExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    public async Task RequestPrices()
    {
        var prices = await _exchangeService.GetAllPricesAsync();
        await Clients.Caller.SendAsync("ReceivePrices", prices);
    }

    public async Task RequestArbitrage()
    {
        var arbitrage = await _exchangeService.DetectArbitrageAsync();
        await Clients.Caller.SendAsync("ReceiveArbitrage", arbitrage);
    }
}
