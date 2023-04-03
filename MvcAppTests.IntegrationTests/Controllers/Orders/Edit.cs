using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Infrastructure.Persistence;
using MvcAppTests.IntegrationTests.Common;

namespace MvcAppTests.IntegrationTests.Controllers.Orders;

public class Edit : IClassFixture<GeneralWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly OrdersController _ordersController;
    private readonly AppDbContext _context;
    private readonly Fixture _fixture;

    public Edit(GeneralWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateAsyncScope();
        _ordersController = _serviceScope.ServiceProvider.GetRequiredService<OrdersController>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Edit_SuccessfullyEditsOrder_IfInputValid()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };
        
        var redirectionResult = await _ordersController.Create(createOrderCommand) as RedirectToActionResult;
        var createdId = (int) redirectionResult!.RouteValues!["id"]!;

        var newNumber = _fixture.Create<string>();
        var updateOrderCommand = createOrderCommand;
        updateOrderCommand.Id = createdId;
        updateOrderCommand.Number = newNumber;

        // Act
        var response = await _ordersController.Edit(updateOrderCommand, createdId);

        // Assert
        var redirectResult = response.Should().BeAssignableTo<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(nameof(OrdersController.Details));
        _context.Orders.Should().Contain(order => order.Id == createdId && order.Number == newNumber);
    }

    [Fact]
    public async Task Edit_PopulatesModelStateWithErrors_IfInputNotValid()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };

        var redirectionResult = await _ordersController.Create(createOrderCommand) as RedirectToActionResult;
        var createdId = (int) redirectionResult!.RouteValues!["id"]!;

        var newNotValidNumber = String.Empty;
        var updateOrderCommand = createOrderCommand;
        updateOrderCommand.Id = createdId;
        updateOrderCommand.Number = newNotValidNumber;

        // Act
        var response = await _ordersController.Edit(updateOrderCommand, createdId);
       
        // Assert
        _ordersController.ModelState.IsValid.Should().BeFalse();
        _ordersController.ModelState.Keys.Should().Contain(nameof(createOrderCommand.Number));
        
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        viewResult.Model.Should().BeEquivalentTo(updateOrderCommand);
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _serviceScope.DisposeAsync();
}