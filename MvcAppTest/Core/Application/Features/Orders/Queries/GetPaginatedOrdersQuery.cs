using Mapster;
using MediatR;
using MvcAppTest.Core.Application.Common;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public record GetPaginatedOrdersQuery(int PageNumber, int PageSize = 10) : IRequest<PaginatedList<OrderVm>>;

public class GetPaginatedOrdersQueryHandler : IRequestHandler<GetPaginatedOrdersQuery, PaginatedList<OrderVm>>
{
    private readonly AppDbContext _context;

    public GetPaginatedOrdersQueryHandler(AppDbContext context) => _context = context;

    public async Task<PaginatedList<OrderVm>> Handle(GetPaginatedOrdersQuery request, CancellationToken token)
    {
        var orderVms = await _context.Orders
            .OrderBy(order => order.Id)
            .ProjectToType<OrderVm>()
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, token: token);

        return orderVms;
    }
}