using MediatR;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.OrderItems.Commands;

public record DeleteOrderItemCommand(int Id) : IRequest<Unit>;

public class DeleteOrderItemCommandHandler : IRequestHandler<DeleteOrderItemCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeleteOrderItemCommandHandler(AppDbContext context) => _context = context;

    public async Task<Unit> Handle(DeleteOrderItemCommand command, CancellationToken token)
    {
        var orderItemPk = new object[] { command.Id };
        var orderItem = await _context.OrderItems.FindAsync(orderItemPk, token);

        _ = orderItem ?? throw new EntityNotFoundException(nameof(OrderItem), command.Id);

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync(token);

        return Unit.Value;
    }
}