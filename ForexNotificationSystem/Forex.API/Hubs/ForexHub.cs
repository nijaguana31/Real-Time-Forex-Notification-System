using Forex.Application.Commands.SubscribeSymbol;
using Forex.Application.Commands.UnsubscribeSymbol;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Forex.API.Hubs
{
    [Authorize] // JWT will be added in next step
    public class ForexHub : Hub
    {
        private readonly IMediator _mediator;

        public ForexHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Subscribe(string symbol)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? Context.ConnectionId;

            await Groups.AddToGroupAsync(Context.ConnectionId, symbol);

            await _mediator.Send(new SubscribeSymbolCommand(userId, symbol));
        }

        public async Task Unsubscribe(string symbol)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);

            await _mediator.Send(new UnsubscribeSymbolCommand(userId, symbol));
        }
    }

}
