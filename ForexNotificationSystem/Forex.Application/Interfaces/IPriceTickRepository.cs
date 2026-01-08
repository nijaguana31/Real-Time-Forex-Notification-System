using Forex.Domain.Entities;

namespace Forex.Application.Interfaces
{
    public interface IPriceTickRepository
    {
        Task AddAsync(PriceTick tick, CancellationToken cancellationToken);
        Task<List<PriceTick>> GetLatestBySymbolAsync(string symbol, int count, CancellationToken cancellationToken);
    }
}
