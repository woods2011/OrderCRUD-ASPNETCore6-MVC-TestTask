namespace MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;

public record OrderItemVm(int Id, int OrderId, string Name, decimal Quantity, string Unit);