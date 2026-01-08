using Forex.Application.Interfaces;
using Forex.Domain.Entities;
using Forex.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Forex.Infrastructure.Repositories
{
    public class PriceTickRepository : IPriceTickRepository
    {
        private readonly ForexDbContext _dbContext;

        public PriceTickRepository(ForexDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PriceTick tick, CancellationToken cancellationToken)
        {
            _dbContext.PriceTicks.Add(tick);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<PriceTick>> GetLatestBySymbolAsync(
    string symbol,
    int count,
    CancellationToken cancellationToken)
        {
            return await _dbContext.PriceTicks
                .AsNoTracking()
                .Where(x => x.Symbol == symbol)
                .OrderByDescending(x => x.TimestampUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

    }
}
