using Forex.Domain.Entities;

namespace Forex.Application.Interfaces
{
    public interface IFinnhubClient
    {
        Task<PriceTick?> GetQuoteAsync(
            string symbol,
            CancellationToken cancellationToken);
    }
}
