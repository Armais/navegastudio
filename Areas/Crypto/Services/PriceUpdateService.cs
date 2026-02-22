using NavegaStudio.Areas.Crypto.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace NavegaStudio.Areas.Crypto.Services;

public class PriceUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<PriceHub> _hubContext;
    private readonly ILogger<PriceUpdateService> _logger;

    public PriceUpdateService(
        IServiceProvider serviceProvider,
        IHubContext<PriceHub> hubContext,
        ILogger<PriceUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PriceUpdateService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var exchangeService = scope.ServiceProvider.GetRequiredService<ExchangeService>();

                var prices = await exchangeService.GetAllPricesAsync();
                await _hubContext.Clients.All.SendAsync("ReceivePrices", prices, stoppingToken);

                var arbitrage = await exchangeService.DetectArbitrageAsync();
                if (arbitrage.Count > 0)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveArbitrage", arbitrage, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in price update loop");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
