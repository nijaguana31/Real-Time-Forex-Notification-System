using Forex.Domain.Enums;

namespace Forex.Domain.Entities
{
    public class SubscriptionAudit
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = default!;

        public string Symbol { get; set; } = default!;

        public SubscriptionAction Action { get; set; }

        public DateTime ActionAtUtc { get; set; }
    }
}
