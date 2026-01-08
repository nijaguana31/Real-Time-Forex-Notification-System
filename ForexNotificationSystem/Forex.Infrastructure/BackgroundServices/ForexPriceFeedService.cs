using Forex.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace Forex.Infrastructure.BackgroundServices
{
    public class ForexPriceFeedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ForexPriceFeedService> _logger;

        // ✅ Valid Finnhub symbols (Free Tier)
        private static readonly string[] Symbols =
        {
            "AAPL",
            "MSFT",
            "GOOGL",
            "TSLA",
            "BTCUSD",
            "ETHUSD"
        };

        public ForexPriceFeedService(
            IServiceScopeFactory scopeFactory,
            ILogger<ForexPriceFeedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Finnhub price feed service started");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // use ambient Activity.TraceId when available, otherwise create a per-iteration id
                    var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

                    // push CorrelationId into Serilog context so all logs in this block include it
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var finnhubClient = scope.ServiceProvider
                            .GetRequiredService<IFinnhubClient>();

                        var priceRepository = scope.ServiceProvider
                            .GetRequiredService<IPriceTickRepository>();

                        foreach (var symbol in Symbols)
                        {
                            var tick = await finnhubClient
                                .GetQuoteAsync(symbol, stoppingToken);

                            if (tick == null || tick.Price <= 0)
                                continue;

                            await priceRepository
                                .AddAsync(tick, stoppingToken);

                            // structured log with object payload; CorrelationId is included via LogContext
                            _logger.LogInformation("Saved price for {Symbol} {@PriceTick}", tick.Symbol, tick);
                        }

                        _logger.LogDebug("Finnhub price feed iteration completed for symbols {@Symbols}", Symbols);
                    }

                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Finnhub price feed service stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in ForexPriceFeedService");
                throw;
            }
        }
    }
}
