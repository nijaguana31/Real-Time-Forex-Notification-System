using Forex.Application.Interfaces;
using Forex.Domain.Entities;
using Forex.Domain.Enums;
using MediatR;

namespace Forex.Application.Commands.SubscribeSymbol
{
    public class SubscribeSymbolHandler : IRequestHandler<SubscribeSymbolCommand>
    {
        private readonly ISubscriptionAuditRepository _auditRepository;

        public SubscribeSymbolHandler(ISubscriptionAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task Handle(
            SubscribeSymbolCommand request,
            CancellationToken cancellationToken)
        {
            var audit = new SubscriptionAudit
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Symbol = request.Symbol,
                Action = SubscriptionAction.Subscribe,
                ActionAtUtc = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(audit, cancellationToken);
        }
    }
}
