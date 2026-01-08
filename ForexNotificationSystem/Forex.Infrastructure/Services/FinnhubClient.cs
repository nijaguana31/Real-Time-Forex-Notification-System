using Forex.Application.Interfaces;
using Forex.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Forex.Infrastructure.Services
{
    public class FinnhubClient : IFinnhubClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FinnhubClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PriceTick?> GetQuoteAsync(
            string symbol,
            CancellationToken cancellationToken)
        {
            var apiKey = _configuration["Finnhub:ApiKey"];
            var baseUrl = _configuration["Finnhub:BaseUrl"];

            var url = $"{baseUrl}/quote?symbol={symbol}&token={apiKey}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            // Finnhub sometimes returns empty or error responses
            if (!root.TryGetProperty("c", out var priceElement))
                return null;

            decimal price = priceElement.GetDecimal();

            decimal bid = root.TryGetProperty("b", out var bidElement)
                ? bidElement.GetDecimal()
                : price;

            decimal ask = root.TryGetProperty("a", out var askElement)
                ? askElement.GetDecimal()
                : price;

            return new PriceTick
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                Price = price,
                Bid = bid,
                Ask = ask,
                TimestampUtc = DateTime.UtcNow
            };

        }
    }
}
