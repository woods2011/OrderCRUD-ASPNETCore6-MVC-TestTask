using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;

public record GetOrderItemsQuery(int OrderId) : IRequest<List<OrderItemVm>>;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, List<OrderItemVm>>
{
    private readonly AppDbContext _context;

    public GetOrderItemsQueryHandler(AppDbContext context) => _context = context;

    public async Task<List<OrderItemVm>> Handle(GetOrderItemsQuery request, CancellationToken token)
    {
        var isOrderExist = await _context.Orders.AnyAsync(order => order.Id == request.OrderId, token);

        if (!isOrderExist)
            throw new EntityNotFoundException(nameof(Order), request.OrderId);

        var orderItemsVms = await _context.OrderItems
            .Where(orderItem => orderItem.OrderId == request.OrderId)
            .ProjectToType<OrderItemVm>()
            .ToListAsync(token);

        return orderItemsVms;
    }
}