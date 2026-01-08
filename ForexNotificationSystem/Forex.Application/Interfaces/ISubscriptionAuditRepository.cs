using Forex.Domain.Entities;

namespace Forex.Application.Interfaces
{
    public interface ISubscriptionAuditRepository
    {
        Task AddAsync(SubscriptionAudit audit, CancellationToken cancellationToken);
    }
}
