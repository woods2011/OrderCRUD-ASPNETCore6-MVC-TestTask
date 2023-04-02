using MediatR;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Commands;

public record DeleteOrderCommand(int Id) : IRequest<Unit>;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
{
    private readonly AppDbContext _context;

    public DeleteOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<Unit> Handle(DeleteOrderCommand command, CancellationToken token)
    {
        var orderPk = new object[] { command.Id };
        var order = await _context.Orders.FindAsync(orderPk, token);

        _ = order ?? throw new EntityNotFoundException(nameof(Order), command.Id);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(token);

        return Unit.Value;
    }
}