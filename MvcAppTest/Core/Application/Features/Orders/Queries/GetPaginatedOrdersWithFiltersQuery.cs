using Mapster;
using MediatR;
using MvcAppTest.Core.Application.Common;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Common.Extensions;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public record GetPaginatedOrdersWithFiltersQuery(
    int PageNumber,
    int PageSize,
    string[] OrderNumberFilter,
    string[] OrderProviderNameFilter,
    DateTime? OrderDateStartFilter,
    DateTime? OrderDateEndFilter,
    string[] OrderItemNameFilter,
    string[] OrderItemUnitFilter) : IRequest<PaginatedList<OrderVm>>
{
    public int PageSize { get; set; } = PageSize;
}

public class GetPaginatedOrdersWithFiltersQueryHandler :
    IRequestHandler<GetPaginatedOrdersWithFiltersQuery, PaginatedList<OrderVm>>
{
    private readonly AppDbContext _context;

    public GetPaginatedOrdersWithFiltersQueryHandler(AppDbContext context) => _context = context;

    public async Task<PaginatedList<OrderVm>> Handle(
        GetPaginatedOrdersWithFiltersQuery request,
        CancellationToken token)
    {
        var (pageNumber, pageSize,
            orderNumbers, orderProviderNames,
            orderDateStart, orderDateEnd,
            itemNames, itemUnits) = request;

        orderDateStart = orderDateStart?.ToUniversalTime();
        orderDateEnd = orderDateEnd?.ToUniversalTime();

        var orderVms = await _context.Orders
            .WhereIf(order => orderNumbers.Contains(order.Number), orderNumbers.Any())
            .WhereIf(order => orderProviderNames.Contains(order.Provider.Name), orderProviderNames.Any())
            .WhereIf(order => order.Date >= orderDateStart, orderDateStart is not null)
            .WhereIf(order => order.Date <= orderDateEnd, orderDateEnd is not null)
            .WhereIf(order => order.OrderItems.Any(item => itemNames.Contains(item.Name)), itemNames.Any())
            .WhereIf(order => order.OrderItems.Any(item => itemUnits.Contains(item.Unit)), itemUnits.Any())
            .OrderBy(order => order.Id)
            .ProjectToType<OrderVm>()
            .ToPaginatedListAsync(pageNumber, pageSize, token);

        return orderVms;
    }
}