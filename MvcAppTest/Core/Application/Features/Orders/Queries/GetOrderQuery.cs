using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public record GetOrderQuery(int Id) : IRequest<OrderVm>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderVm>
{
    private readonly AppDbContext _context;

    public GetOrderQueryHandler(AppDbContext context) => _context = context;

    public async Task<OrderVm> Handle(GetOrderQuery request, CancellationToken token)
    {
        var orderDeleteProjection = await _context.Orders.Include(order => order.OrderItems)
            .Where(order => order.Id == request.Id)
            .ProjectToType<OrderVm>()
            .FirstOrDefaultAsync(token);
        
        _ = orderDeleteProjection ?? throw new EntityNotFoundException(nameof(Order), request.Id);
        
        return orderDeleteProjection;
    }
}
