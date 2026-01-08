using Forex.Application.DTOs;
using MediatR;

namespace Forex.Application.Queries.GetLatestPrices
{
    public record GetLatestPricesQuery(string Symbol, int Count)
        : IRequest<List<PriceTickDto>>;
}
