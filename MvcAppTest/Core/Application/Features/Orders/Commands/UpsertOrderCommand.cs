using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.Commands;

public class UpsertOrderCommand : IRequest<int>
{
    public int? Id { get; set; }
    public string Number { get; set; } = String.Empty;
    public DateTime Date { get; set; }
    public int ProviderId { get; set; }
}

public class UpsertOrderCommandHandler : IRequestHandler<UpsertOrderCommand, int>
{
    private readonly AppDbContext _context;

    public UpsertOrderCommandHandler(AppDbContext context) => _context = context;

    public async Task<int> Handle(UpsertOrderCommand command, CancellationToken token)
    {
        if (command.Id is null)
            return await CreateOrder(command, token);
        
        var order = await _context.Orders.Include(order => order.OrderItems)
            .FirstOrDefaultAsync(order => order.Id == command.Id, token);

        _ = order ?? throw new EntityNotFoundException(nameof(Order), command.Id.Value);

        order.UpdateNumber(command.Number);
        order.Date = command.Date.ToUniversalTime();
        order.ProviderId = command.ProviderId;

        await _context.SaveChangesAsync(token);
        return order.Id;
        
        
        async Task<int> CreateOrder(UpsertOrderCommand createCommand, CancellationToken ct)
        {
            var newOrder = new Order(
                createCommand.Number,
                createCommand.Date.ToUniversalTime(),
                createCommand.ProviderId);

            await _context.Orders.AddAsync(newOrder, ct);

            await _context.SaveChangesAsync(ct);
            return newOrder.Id;
        }
    }
}

public class UpsertOrderCommandValidator : AbstractValidator<UpsertOrderCommand>
{
    public UpsertOrderCommandValidator(AppDbContext context)
    {
        RuleFor(command => command.Number).NotEmpty();
        RuleFor(command => command.Date).NotEmpty().GreaterThan(new DateTime(1900, 1, 1));
        RuleFor(command => command.ProviderId).NotEmpty();

        RuleFor(command => command.Number).MustAsync(async (command, number, token) =>
        {
            var isExists = await context.Orders
                .Where(order => order.Id != command.Id)
                .Where(order => order.Number == number && order.ProviderId == command.ProviderId)
                .AnyAsync(token);
            return isExists is false;
        }).WithMessage("Заказ с таким номером и поставщиком уже существует");
    }
}