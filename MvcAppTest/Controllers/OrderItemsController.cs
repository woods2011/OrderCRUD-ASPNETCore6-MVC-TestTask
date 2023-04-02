using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MvcAppTest.Core.Application.Features.Orders.OrderItems.Commands;
using MvcAppTest.Core.Application.Features.Orders.OrderItems.Queries;

namespace MvcAppTest.Controllers;

[Route("Orders/{orderId:int}/Items")]
public class OrderItemsController : MvcController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public OrderItemsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> Index(int orderId)
    {
        var orderItemVms = await _mediator.Send(new GetOrderItemsQuery(orderId));
        
        return View(orderItemVms);
    }


    [HttpGet("Create")]
    public IActionResult Create(int orderId)
    {
        var upsertOrderItemCommand = new UpsertOrderItemCommand { OrderId = orderId };
        
        return View(upsertOrderItemCommand);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(int orderId, UpsertOrderItemCommand upsertOrderItemCommand)
    {
        try
        {
            await _mediator.Send(upsertOrderItemCommand);
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }
        catch (Exception domainOrValidationEx)
        {
            if (!HandleDomainOrValidationException(domainOrValidationEx)) throw;
        }

        return View(upsertOrderItemCommand);
    }


    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var orderItemVm = await _mediator.Send(new GetOrderItemQuery(id));
        var upsertOrderItemCommand = _mapper.Map<UpsertOrderItemCommand>(orderItemVm);
        
        return View(upsertOrderItemCommand);
    }

    [HttpPost("Edit/{id:int}")]
    public async Task<IActionResult> Edit(UpsertOrderItemCommand upsertOrderItemCommand)
    {
        try
        {
            await _mediator.Send(upsertOrderItemCommand);
            return RedirectToAction("Details", "Orders", new { id = upsertOrderItemCommand.OrderId });
        }
        catch (Exception domainOrValidationEx)
        {
            if (!HandleDomainOrValidationException(domainOrValidationEx)) throw;
        }

        return View(upsertOrderItemCommand);
    }


    [HttpGet("Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var orderItemVm = await _mediator.Send(new GetOrderItemQuery(id));

        return View(orderItemVm);
    }


    [HttpGet("Delete/{id:int}/")]
    public async Task<IActionResult> Delete(int id)
    {
        var orderItemVm = await _mediator.Send(new GetOrderItemQuery(id));

        return View(orderItemVm);
    }

    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int orderId, int id)
    {
        await _mediator.Send(new DeleteOrderItemCommand(id));

        return RedirectToAction("Details", "Orders", new { id = orderId });
    }
}