using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MvcAppTest.Core.Domain;

namespace MvcAppTest.Infrastructure.Persistence.Configurations;

public class OrderItemConfigurations  : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem");
        
        builder.Property(orderItem => orderItem.Quantity).HasColumnType("decimal(18,3)");
        
        // тут хорошо было бы добавить индексы т.к. используем для фильтрации, но тогда столбцы будут nvarchar(450), а не nvarchar(max) из условия задания
        // builder.HasIndex(order => order.Name);
        // builder.HasIndex(order => order.Unit);
    }
}