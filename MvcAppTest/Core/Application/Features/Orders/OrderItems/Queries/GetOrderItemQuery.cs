using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;

public record GetOrderItemQuery(int Id) : IRequest<OrderItemVm>;

public class GetOrderItemQueryHandler : IRequestHandler<GetOrderItemQuery, OrderItemVm>
{
    private readonly AppDbContext _context;

    public GetOrderItemQueryHandler(AppDbContext context) => _context = context;

    public async Task<OrderItemVm> Handle(GetOrderItemQuery request, CancellationToken token)
    {
        var orderItemVm = await _context.OrderItems
            .Where(orderItem => orderItem.Id == request.Id)
            .ProjectToType<OrderItemVm>()
            .FirstOrDefaultAsync(token);
        
        if (orderItemVm is null) 
            throw new EntityNotFoundException(nameof(OrderItem), request.Id);

        return orderItemVm;
    }
}