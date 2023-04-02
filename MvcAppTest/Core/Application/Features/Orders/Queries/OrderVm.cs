namespace MvcAppTest.Core.Application.Features.Orders.Queries;

public record OrderVm(int Id, string Number, DateTime Date, int OrderItemsCount, int ProviderId, string ProviderName)
{
    public int OrderItemsCount { get; set; } = OrderItemsCount;
};