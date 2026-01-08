using Forex.Application.Interfaces;
using Forex.Domain.Entities;
using Forex.Domain.Enums;
using MediatR;

namespace Forex.Application.Commands.UnsubscribeSymbol
{
    public class UnsubscribeSymbolHandler : IRequestHandler<UnsubscribeSymbolCommand>
    {
        private readonly ISubscriptionAuditRepository _auditRepository;

        public UnsubscribeSymbolHandler(ISubscriptionAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task Handle(
            UnsubscribeSymbolCommand request,
            CancellationToken cancellationToken)
        {
            var audit = new SubscriptionAudit
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Symbol = request.Symbol,
                Action = SubscriptionAction.Unsubscribe,
                ActionAtUtc = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(audit, cancellationToken);
        }
    }
}
