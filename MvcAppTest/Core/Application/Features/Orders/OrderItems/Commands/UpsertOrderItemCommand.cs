using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure;
using MvcAppTest.Infrastructure.Persistence;

namespace MvcAppTest.Core.Application.Features.Orders.OrderItems.Commands;

public record UpsertOrderItemCommand : IRequest<int>
{
    public int? Id { get; set; }
    public int OrderId { get; set; }
    public string Name { get; set; } = String.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = String.Empty;
}

public class UpsertOrderItemCommandHandler : IRequestHandler<UpsertOrderItemCommand, int>
{
    private readonly AppDbContext _context;

    public UpsertOrderItemCommandHandler(AppDbContext context) => _context = context;

    public async Task<int> Handle(UpsertOrderItemCommand command, CancellationToken token)
    {
        if (command.Id is null)
            return await CreateOrderItem(command, token);

        var orderItemPk = new object[] { command.Id };
        var orderItem = await _context.OrderItems.FindAsync(orderItemPk, token);
        _ = orderItem ?? throw new EntityNotFoundException(nameof(OrderItem), command.Id.Value);

        var orderNumber = await _context.Orders
            .Where(order => order.Id == command.OrderId)
            .Select(order => order.Number)
            .FirstAsync(token);
        
        orderItem.UpdateName(command.Name, orderNumber);
        orderItem.Quantity = command.Quantity;
        orderItem.Unit = command.Unit;

        await _context.SaveChangesAsync(token);
        return orderItem.Id;


        async Task<int> CreateOrderItem(UpsertOrderItemCommand createCommand, CancellationToken ct)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == createCommand.OrderId, ct);
            _ = order ?? throw new EntityNotFoundException(nameof(Order), createCommand.OrderId);

            var newOrderItem = new OrderItem(
                createCommand.OrderId,
                createCommand.Name,
                createCommand.Quantity,
                createCommand.Unit);

            order.AddOrderItem(newOrderItem);

            await _context.SaveChangesAsync(ct);
            return newOrderItem.Id;
        }
    }
}

public class UpsertOrderItemCommandValidator : AbstractValidator<UpsertOrderItemCommand>
{
    public UpsertOrderItemCommandValidator(AppDbContext context)
    {
        RuleFor(command => command.Name).NotEmpty();
        RuleFor(command => command.Quantity).GreaterThan(0);
        RuleFor(command => command.Unit).NotEmpty();

        RuleFor(command => command.Name).MustAsync(async (command, name, token) =>
        {
            var orderNumber = await context.Orders
                .Where(order => order.Id == command.OrderId)
                .Select(order => order.Number).FirstOrDefaultAsync(token);
        
            _ = orderNumber ?? throw new EntityNotFoundException(nameof(Order), command.OrderId);
        
            return name != orderNumber;
        }).WithMessage("Название элемента заказа не может совпадать с номером заказа");

        // Вообще говоря эту проверку можно убрать, т.к. она выполняется в доменном слое, но она позволяет заполнить ModelState
    }
}