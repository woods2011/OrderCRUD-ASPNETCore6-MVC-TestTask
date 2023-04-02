using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MvcAppTest.Core.Application.Common;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;
using MvcAppTest.Core.Application.Features.Orders.Queries;
using MvcAppTest.Core.Application.Features.Providers;
using MvcAppTest.Infrastructure.Common.Extensions;
using MvcAppTest.ViewModels;

namespace MvcAppTest.Controllers;

public class OrdersController : MvcController
{
    private readonly ISender _mediatr;
    private readonly IMapper _mapper;
    private readonly IValidator<ComposedIndexViewModel> _indexVmValidator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly int _defaultPageSize;


    public OrdersController(
        ISender mediatr,
        IMapper mapper,
        IValidator<ComposedIndexViewModel> indexVmValidator,
        IDateTimeProvider dateTimeProvider,
        IOptions<PaginationOptions> paginationOptions)
    {
        _mediatr = mediatr;
        _mapper = mapper;
        _indexVmValidator = indexVmValidator;
        _dateTimeProvider = dateTimeProvider;
        _defaultPageSize = paginationOptions.Value.PageSize;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ComposedIndexViewModel indexVm)
    {
        var allFiltersDistinctVm = await _mediatr.Send(new GetAllFiltersDistinctQuery());
        ViewData[ViewDataKeys.AllFiltersDistinct] = allFiltersDistinctVm;

        if (indexVm.FilterOn is 0)
        {
            var query = new GetPaginatedOrdersQuery(indexVm.Page, _defaultPageSize);
            indexVm.Orders = await _mediatr.Send(query);

            return View(indexVm);
        }

        var validationResult = await _indexVmValidator.ValidateAsync(indexVm);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);

            return View(indexVm);
        }

        var queryWithFilters = _mapper.Map<GetPaginatedOrdersWithFiltersQuery>(indexVm);
        queryWithFilters.PageSize = _defaultPageSize;

        var orderVmsPaginatedList = await _mediatr.Send(queryWithFilters);
        indexVm.Orders = orderVmsPaginatedList;

        return View(indexVm);
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var providers = await _mediatr.Send(new GetProvidersSelectInfoQuery());

        ViewData[ViewDataKeys.Providers] = new SelectList(
            providers, nameof(ProviderSelectInfoVm.Id), nameof(ProviderSelectInfoVm.Name));

        return View(new UpsertOrderCommand { Date = _dateTimeProvider.Now.RoundToSeconds() });
    }

    [HttpPost]
    public async Task<IActionResult> Create(UpsertOrderCommand upsertOrderCommand)
    {
        try
        {
            var createdId = await _mediatr.Send(upsertOrderCommand);

            return RedirectToAction(nameof(Details), new { id = createdId });
        }
        catch (Exception domainOrValidationEx)
        {
            if (!HandleDomainOrValidationException(domainOrValidationEx)) throw;
        }

        var providers = await _mediatr.Send(new GetProvidersSelectInfoQuery());
        ViewData[ViewDataKeys.Providers] = new SelectList(
            providers, nameof(ProviderSelectInfoVm.Id), nameof(ProviderSelectInfoVm.Name));

        return View(upsertOrderCommand);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var orderWithItemsVm = await _mediatr.Send(new GetOrderWithItemsQuery(id));

        var upsertOrderCommand = _mapper.Map<UpsertOrderCommand>(orderWithItemsVm.OrderVm);
        var providers = await _mediatr.Send(new GetProvidersSelectInfoQuery());

        ViewData[ViewDataKeys.OrderItemsVms] = orderWithItemsVm.OrderItemsVms;
        ViewData[ViewDataKeys.Providers] = new SelectList(
            providers, nameof(ProviderSelectInfoVm.Id), nameof(ProviderSelectInfoVm.Name));

        return View(upsertOrderCommand);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpsertOrderCommand upsertOrderCommand, int id)
    {
        try
        {
            await _mediatr.Send(upsertOrderCommand);

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception domainOrValidationEx)
        {
            if (!HandleDomainOrValidationException(domainOrValidationEx)) throw;
        }

        var orderItemsVms = await _mediatr.Send(new GetOrderItemsQuery(id));
        var providers = await _mediatr.Send(new GetProvidersSelectInfoQuery());

        ViewData[ViewDataKeys.OrderItemsVms] = orderItemsVms;
        ViewData[ViewDataKeys.Providers] = new SelectList(
            providers, nameof(ProviderSelectInfoVm.Id), nameof(ProviderSelectInfoVm.Name));

        return View(upsertOrderCommand);
    }


    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var orderDetailsWithItemsVm = await _mediatr.Send(new GetOrderWithItemsQuery(id));

        return View(orderDetailsWithItemsVm);
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var orderVm = await _mediatr.Send(new GetOrderQuery(id));

        return View(orderVm);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _mediatr.Send(new DeleteOrderCommand(id));

        return RedirectToAction(nameof(Index));
    }

    public static class ViewDataKeys
    {
        public const string AllFiltersDistinct = nameof(AllFiltersDistinct);
        public const string OrderItemsVms = nameof(OrderItemsVms);
        public const string Providers = nameof(Providers);
    }
}

public class PaginationOptions
{
    public const string SectionName = "PaginationOptions";

    [Range(3, 100)]
    public int PageSize { get; set; } = 10;
}

// [Bind(Prefix = nameof(UpsertOrderCommand))] UpsertOrderCommand command