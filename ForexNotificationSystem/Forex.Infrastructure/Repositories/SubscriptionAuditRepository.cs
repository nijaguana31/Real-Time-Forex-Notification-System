using Forex.Application.Interfaces;
using Forex.Domain.Entities;
using Forex.Infrastructure.Persistence.DbContext;

namespace Forex.Infrastructure.Repositories
{
    public class SubscriptionAuditRepository : ISubscriptionAuditRepository
    {
        private readonly ForexDbContext _dbContext;

        public SubscriptionAuditRepository(ForexDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SubscriptionAudit audit, CancellationToken cancellationToken)
        {
            _dbContext.SubscriptionAudits.Add(audit);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
