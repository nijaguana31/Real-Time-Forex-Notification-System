using Forex.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace Forex.Infrastructure.BackgroundServices
{
    public class PriceBroadcastService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<Hub> _hubContext;
        private readonly ILogger<PriceBroadcastService> _logger;

        // Same valid symbols
        private static readonly string[] Symbols =
        {
            "AAPL",
            "MSFT",
            "GOOGL",
            "TSLA",
            "BTCUSD",
            "ETHUSD"
        };

        public PriceBroadcastService(
            IServiceScopeFactory scopeFactory,
            IHubContext<Hub> hubContext,
            ILogger<PriceBroadcastService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PriceBroadcastService starting");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Derive correlation id from ambient Activity (if present) otherwise create one per iteration
                    var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

                    // Use matching `using(...)` forms so the block that follows binds correctly
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var priceRepository = scope.ServiceProvider
                            .GetRequiredService<IPriceTickRepository>();

                        foreach (var symbol in Symbols)
                        {
                            var latest = await priceRepository
                                .GetLatestBySymbolAsync(symbol, 1, stoppingToken);

                            if (latest.Count == 0)
                                continue;

                            var tick = latest[0];

                            await _hubContext.Clients
                                .Group(symbol)
                                .SendAsync("PriceUpdate", tick, stoppingToken);

                            // Structured log: includes CorrelationId via LogContext
                            _logger.LogInformation("Broadcasted price for {Symbol} {@PriceTick}", symbol, tick);
                        }
                    }

                    await Task.Delay(500, stoppingToken); // broadcast interval
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PriceBroadcastService stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in PriceBroadcastService");
                throw;
            }
        }
    }
}
