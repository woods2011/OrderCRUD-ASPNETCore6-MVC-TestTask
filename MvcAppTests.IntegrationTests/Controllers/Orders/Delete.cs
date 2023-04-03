using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Common.Exceptions;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Infrastructure.Persistence;
using MvcAppTests.IntegrationTests.Common;

namespace MvcAppTests.IntegrationTests.Controllers.Orders;

public class Delete : IClassFixture<GeneralWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly OrdersController _ordersController;
    private readonly AppDbContext _context;
    private readonly Fixture _fixture;

    public Delete(GeneralWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateAsyncScope();
        _ordersController = _serviceScope.ServiceProvider.GetRequiredService<OrdersController>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_SuccessfullyDeletesOrder_IfOrderExists()
    {
        // Arrange
        var createOrderCommand =
            new UpsertOrderCommand { Date = DateTime.Now, Number = _fixture.Create<string>(), ProviderId = 1 };

        var redirectionResult = await _ordersController.Create(createOrderCommand) as RedirectToActionResult;
        var createdId = (int) redirectionResult!.RouteValues!["id"]!;
        var orderExistedAfterCreating = await _context.Orders.AnyAsync(order => order.Id == createdId);

        // Act
        var response = await _ordersController.DeleteConfirmed(createdId);

        // Assert
        var redirectResult = response.Should().BeAssignableTo<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(nameof(OrdersController.Index));
        orderExistedAfterCreating.Should().BeTrue();
        _context.Orders.Should().NotContain(order => order.Id == createdId);
    }

    [Fact]
    public async Task Delete_PopulatesModelStateWithErrors_IfOrderNotExist()
    {
        // Arrange
        var orderIdToDeleted = Int32.MaxValue;

        // Act
        var act = async () => await _ordersController.DeleteConfirmed(orderIdToDeleted);

        // Assert
        var exception = await act.Should().ThrowAsync<EntityNotFoundException>();
        exception.Which.Message.Should().Contain(orderIdToDeleted.ToString());
    }


    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _serviceScope.DisposeAsync();
}