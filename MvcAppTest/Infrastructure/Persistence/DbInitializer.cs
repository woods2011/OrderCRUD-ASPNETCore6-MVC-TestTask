using Bogus;
using MvcAppTest.Core.Domain;
using MvcAppTest.Infrastructure.Common.Extensions;

namespace MvcAppTest.Infrastructure.Persistence;

public interface IDbInitializer
{
    void Seed(AppDbContext context);
}

public class DbInitializer : IDbInitializer
{
    public void Seed(AppDbContext context)
    {
        if (context.Providers.Any()) return;

        var faker = new Faker("ru");
        const int providesCount = 15;
        const int ordersCount = 50;
        const int orderItemsCount = ordersCount * 3;


        var providers = Enumerable.Range(1, providesCount).Select(i => new Provider($"Поставщик {i}"));

        context.Providers.AddRange(providers);
        context.SaveChanges();
        var providersIds = context.Providers.Select(p => p.Id).ToList();


        var orders = Enumerable.Range(1, ordersCount).Select(i => new Order(
            number: $"Номер {i}",
            date: i < 15 ? faker.Date.Recent(20).RoundToSeconds() : faker.Date.Recent(60).RoundToSeconds(),
            providerId: faker.PickRandom(providersIds)));

        context.Orders.AddRange(orders);
        context.SaveChanges();
        var ordersIds = context.Orders.Select(o => o.Id).ToList();


        var orderItems = Enumerable.Range(1, orderItemsCount).Select(i => new OrderItem(
            orderId: faker.PickRandom(ordersIds),
            name: $"Элемент {faker.Random.Int(1, 20)}",
            quantity: faker.Random.Decimal(0.001m, 100),
            unit: faker.PickRandom("т", "кг", "г", "мг", "мкг")));

        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }
}