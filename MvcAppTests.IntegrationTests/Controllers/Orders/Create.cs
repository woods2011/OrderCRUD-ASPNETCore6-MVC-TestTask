using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Infrastructure.Persistence;
using MvcAppTests.IntegrationTests.Common;

namespace MvcAppTests.IntegrationTests.Controllers.Orders;

public class Create : IClassFixture<GeneralWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly OrdersController _ordersController;
    private readonly AppDbContext _context;
    private readonly Fixture _fixture;

    public Create(GeneralWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateAsyncScope();
        _ordersController = _serviceScope.ServiceProvider.GetRequiredService<OrdersController>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_SuccessfullyCreatesOrder_IfInputValid()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };

        // Act
        var response = await _ordersController.Create(createOrderCommand);

        // Assert
        var redirectResult = response.Should().BeAssignableTo<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(nameof(OrdersController.Details));
        
        var orderId = redirectResult.RouteValues!["id"].Should().BeAssignableTo<int>().Subject;
        _context.Orders.Should().Contain(order => order.Id == orderId);
    }

    [Fact]
    public async Task Create_PopulatesModelStateWithErrors_IfInputNotValid()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = String.Empty, ProviderId = 1 };

        // Act
        var response = await _ordersController.Create(createOrderCommand);
        
        // Assert
        _ordersController.ModelState.IsValid.Should().BeFalse();
        _ordersController.ModelState.Keys.Should().Contain(nameof(createOrderCommand.Number));
        
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        viewResult.Model.Should().BeEquivalentTo(createOrderCommand);
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _serviceScope.DisposeAsync();
}