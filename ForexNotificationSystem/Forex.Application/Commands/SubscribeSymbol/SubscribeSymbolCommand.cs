using MediatR;

namespace Forex.Application.Commands.SubscribeSymbol
{
    public record SubscribeSymbolCommand(string UserId, string Symbol) : IRequest;
}
