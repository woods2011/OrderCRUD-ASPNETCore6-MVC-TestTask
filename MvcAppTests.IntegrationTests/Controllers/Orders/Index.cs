using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MvcAppTest.Controllers;
using MvcAppTest.Core.Application.Features.Orders.Commands;
using MvcAppTest.Infrastructure.Persistence;
using MvcAppTest.ViewModels;
using MvcAppTests.IntegrationTests.Common;

namespace MvcAppTests.IntegrationTests.Controllers.Orders;

public class Index : IClassFixture<GeneralWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _serviceScope;
    private readonly OrdersController _ordersController;
    private readonly AppDbContext _context;

    public Index(GeneralWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateAsyncScope();
        _ordersController = _serviceScope.ServiceProvider.GetRequiredService<OrdersController>();
        _context = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Index_ReturnsOrders_IfTheyExists()
    {
        // Arrange
        await _ordersController.Create(new UpsertOrderCommand { Date = DateTime.Now, Number = "123", ProviderId = 1 });

        // Act
        var response = await _ordersController.Index(new ComposedIndexViewModel());

        // Assert
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        var indexVm = viewResult.Model.Should().BeAssignableTo<ComposedIndexViewModel>().Subject;
        indexVm.Orders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Index_ReturnsEmptyOrdersList_IfTheyNotExists()
    {
        // Arrange
        _context.Orders.RemoveRange(_context.Orders);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _ordersController.Index(new ComposedIndexViewModel());

        // Assert
        var viewResult = response.Should().BeAssignableTo<ViewResult>().Subject;
        var indexVm = viewResult.Model.Should().BeAssignableTo<ComposedIndexViewModel>().Subject;
        indexVm.Orders.Should().BeEmpty();
    }

    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _serviceScope.DisposeAsync();
}