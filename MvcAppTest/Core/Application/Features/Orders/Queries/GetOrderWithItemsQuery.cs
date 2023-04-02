using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public record GetOrderWithItemsQuery(int Id) : IRequest<OrderWithItemsVm>;

public class GetOrderWithItemsQueryHandler : IRequestHandler<GetOrderWithItemsQuery, OrderWithItemsVm>
{
    private readonly AppDbContext _context;

    public GetOrderWithItemsQueryHandler(AppDbContext context) => _context = context;

    public async Task<OrderWithItemsVm> Handle(GetOrderWithItemsQuery request, CancellationToken token)
    {
        var orderProjection = await _context.Orders
            .Include(order => order.OrderItems)
            .Where(order => order.Id == request.Id)
            .ProjectToType<OrderWithItemsVm>()
            .FirstOrDefaultAsync(token);

        _ = orderProjection ?? throw new EntityNotFoundException(nameof(Order), request.Id);

        orderProjection.OrderVm.OrderItemsCount = orderProjection.OrderItemsVms.Count; // After mapping not working
        return orderProjection;
    }
}

public record OrderWithItemsVm(OrderVm OrderVm, List<OrderItemVm> OrderItemsVms);