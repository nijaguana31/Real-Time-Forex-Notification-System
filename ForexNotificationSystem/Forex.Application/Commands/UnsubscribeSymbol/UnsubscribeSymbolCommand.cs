using MediatR;

namespace Forex.Application.Commands.UnsubscribeSymbol
{
    public record UnsubscribeSymbolCommand(string UserId, string Symbol) : IRequest;
}
