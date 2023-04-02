using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MvcAppTest.Core.Domain;

namespace MvcAppTest.Infrastructure.Persistence.Configurations;

public class ProviderConfigurations : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("Provider");

        // тут хорошо было бы добавить индекс т.к. используем для фильтрации, но тогда столбец будет nvarchar(450), а не nvarchar(max) из условия задания
        // builder.HasIndex(provider => provider.Name);
    }
}