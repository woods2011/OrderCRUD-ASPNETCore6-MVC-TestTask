using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MvcAppTest.Core.Domain;

namespace MvcAppTest.Infrastructure.Persistence.Configurations;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        
        builder.Property(order => order.Date).HasColumnType("datetime2(7)");
        
        // тут хорошо было бы добавить индекс т.к. используем для фильтрации, но тогда столбец будет nvarchar(450), а не nvarchar(max) из условия задания
        // builder.HasIndex(order => order.Number);
        builder.HasIndex(order => order.Date);
        
        builder.Metadata.FindNavigation(nameof(Order.OrderItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}