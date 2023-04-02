using MvcAppTest.Core.Domain.Exceptions;

namespace MvcAppTest.Core.Domain;

public class Order
{
    private readonly List<OrderItem> _orderItems = new();

    public int Id { get; private set; }

    public string Number { get; private set; }

    public DateTime Date { get; set; }

    public IEnumerable<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public int ProviderId { get; set; }

    public Provider Provider { get; private set; } = null!;


    public Order(
        string number,
        DateTime date,
        IEnumerable<OrderItem> orderItems,
        int providerId)
    {
        Number = number;
        Date = date;
        ProviderId = providerId;

        foreach (var orderItem in orderItems) AddOrderItem(orderItem);
    }

    public Order(string number, DateTime date, int providerId) :
        this(number, date, Enumerable.Empty<OrderItem>(), providerId) { }

    
    public void UpdateNumber(string newNumber)
    {
        if (Number == newNumber) return;
        
        if (OrderItems.Any(item => item.Name == newNumber))
            throw new DomainException("Номер заказа не может совпадать с названием одного из элементов заказа");

        Number = newNumber;
    }

    public void AddOrderItem(OrderItem item)
    {
        if (item.Name == Number)
            throw new DomainException("Название элемента заказа не может совпадать с номером заказа");

        _orderItems.Add(item);
    }


#pragma warning disable CS8618
    private Order() { }
#pragma warning restore CS8618
}