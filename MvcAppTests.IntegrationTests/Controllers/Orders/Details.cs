using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Core.Application.Features.Orders.OrderItems.Commands;
using MvcAppTest.Core.Application.Features.Orders.Queries;
using MvcAppTest.Infrastructure.Persistence;
using MvcAppTests.IntegrationTests.Common;

namespace MvcAppTests.IntegrationTests.Controllers.Orders;

public class Details : IClassFixture<GeneralWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly OrdersController _ordersController;
    private readonly OrderItemsController _ordersItemsController;
    private readonly Fixture _fixture;

    public Details(GeneralWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateAsyncScope();
        _ordersController = _serviceScope.ServiceProvider.GetRequiredService<OrdersController>();
        _ordersItemsController = _serviceScope.ServiceProvider.GetRequiredService<OrderItemsController>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Details_ReturnsOrderDetails_IfOrderExists()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };
        
        var redirectionResult = await _ordersController.Create(createOrderCommand) as RedirectToActionResult;
        var createdId = (int) redirectionResult!.RouteValues!["id"]!;

        // Act
        var response = await _ordersController.Details(createdId);

        // Assert
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        
        var model = viewResult.Model as OrderWithItemsVm;
        model.Should().NotBeNull();
        model!.OrderVm.Id.Should().Be(createdId);
        model.OrderItemsVms.Should().BeEmpty();
    }

    [Fact]
    public async Task Details_ReturnsOrderWithOrderItemsDetails_IfOrderExistsWithItems()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };
        
        var redirectionResult = await _ordersController.Create(createOrderCommand) as RedirectToActionResult;
        var createdId = (int) redirectionResult!.RouteValues!["id"]!;

        var orderItemsCount = 2;
        var createOrderItemsCommands = _fixture.Build<UpsertOrderItemCommand>()
            .With(x => x.Id, null as int?)
            .With(x => x.OrderId, createdId)
            .With(x => x.Quantity, 1)
            .CreateMany(orderItemsCount);
        
        foreach (var createOrderItemCommand in createOrderItemsCommands)
            await _ordersItemsController.Create(createdId, createOrderItemCommand);

        // Act
        var response = await _ordersController.Details(createdId);

        // Assert
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        
        var model = viewResult.Model as OrderWithItemsVm;
        model.Should().NotBeNull();
        model!.OrderVm.Id.Should().Be(createdId);
        model.OrderItemsVms.Should().HaveCount(orderItemsCount);
        model.OrderItemsVms.Should().AllSatisfy(itemVm => itemVm.OrderId.Should().Be(createdId));
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _serviceScope.DisposeAsync();
}