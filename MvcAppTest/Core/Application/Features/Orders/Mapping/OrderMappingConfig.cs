using Mapster;
using MvcAppTest.Core.Application.Features.Orders.Queries;
using MvcAppTest.Core.Domain;
using MvcAppTest.ViewModels;

namespace MvcAppTest.Core.Application.Features.Orders.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Order, OrderWithItemsVm>()
            .Map(dest => dest.OrderVm, src => src)
            .Map(dest => dest.OrderVm.Date, src => src.Date.ToLocalTime())
            .Map(dest => dest.OrderVm.OrderItemsCount, src => 0)
            .Map(dest => dest.OrderItemsVms, src => src.OrderItems);

        config.ForType<Order, OrderVm>()
            .Map(dest => dest.Date, src => src.Date.ToLocalTime())
            .Map(dest => dest.OrderItemsCount, src => src.OrderItems.Count());
        
        // Этот маппинг должен быть в другой папке на уровне представления
        config.ForType<ComposedIndexViewModel, GetPaginatedOrdersWithFiltersQuery>()
            .Map(dest => dest.PageNumber, src => src.Page)
            .Map(dest => dest.PageSize, src => 10)
            .Map(dest => dest.OrderNumberFilter, src => src.OrdNumber)
            .Map(dest => dest.OrderProviderNameFilter, src => src.OrdProvider)
            .Map(dest => dest.OrderDateStartFilter, src => src.OrdStart)
            .Map(dest => dest.OrderDateEndFilter, src => src.OrdEnd.AddDays(1))
            .Map(dest => dest.OrderItemNameFilter, src => src.OrdItemName)
            .Map(dest => dest.OrderItemUnitFilter, src => src.OrdItemUnit);
    }
}