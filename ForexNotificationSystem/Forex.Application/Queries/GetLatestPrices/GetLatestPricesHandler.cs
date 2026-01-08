using Forex.Application.DTOs;
using Forex.Application.Interfaces;
using MediatR;

namespace Forex.Application.Queries.GetLatestPrices
{
    public class GetLatestPricesHandler
        : IRequestHandler<GetLatestPricesQuery, List<PriceTickDto>>
    {
        private readonly IPriceTickRepository _repository;

        public GetLatestPricesHandler(IPriceTickRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PriceTickDto>> Handle(
            GetLatestPricesQuery request,
            CancellationToken cancellationToken)
        {
            var ticks = await _repository
                .GetLatestBySymbolAsync(request.Symbol, request.Count, cancellationToken);

            return ticks
                .Select(t => new PriceTickDto
                {
                    Symbol = t.Symbol,
                    Price = t.Price,
                    Bid = t.Bid,
                    Ask = t.Ask,
                    TimestampUtc = t.TimestampUtc
                })
                .ToList();
        }
    }
}
