using MvcAppTest.Core.Domain.Exceptions;

namespace MvcAppTest.Core.Domain;

public class OrderItem
{
    public int Id { get; private set; }

    public int OrderId { get; private set; }

    public string Name { get; private set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; }

    public OrderItem(int orderId, string name, decimal quantity, string unit)
    {
        OrderId = orderId;
        Name = name;
        Quantity = quantity;
        Unit = unit;
    }

    public void UpdateName(string newName, string orderNumber)
    {
        if (newName == orderNumber)
            throw new DomainException("Название элемента заказа не может совпадать с номером заказа");

        Name = newName;
    }

#pragma warning disable CS8618
    private OrderItem() { }
#pragma warning restore CS8618
}