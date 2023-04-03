using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using MvcAppTest.Core.Domain;
using MvcAppTest.Core.Domain.Exceptions;

namespace MvcAppTests.UnitTests;

public class OrderTests
{
    [Theory, AutoData]
    public void AddOrderItem_ThrowDomainException_WhenOrderItemNameEqualToOrderNumber(Order sut)
    {
        // Arrange
        var orderItem = new OrderItem(sut.Id, sut.Number, 1, "шт.");

        // Act
        var act = () => sut.AddOrderItem(orderItem);

        // Assert
        act.Should().Throw<DomainException>();
    }
    
    [Theory, AutoData]
    public void AddOrderItem_AddsItemSuccessfully_WhenOrderItemNameIsValid(Order sut)
    {
        // Arrange
        var orderItem = new OrderItem(sut.Id, $"not{sut.Number}", 1, "шт.");

        // Act
        var act = () => sut.AddOrderItem(orderItem);

        // Assert
        act.Should().NotThrow();
        sut.OrderItems.Should().Contain(orderItem);
    }
    

    [Theory, AutoData]
    public void UpdateNumber_ThrowDomainException_WhenNewValueEqualToOneOfOrderItemsNames(
        Order sut,
        string newOrderNumber)
    {
        // Arrange
        var orderItem = new OrderItem(sut.Id, newOrderNumber, 1, "шт.");
        sut.AddOrderItem(orderItem);

        // Act
        var act = () => sut.UpdateNumber(newOrderNumber);

        // Assert
        act.Should().Throw<DomainException>();
    }


    [Theory, AutoData]
    public void UpdateNumber_UpdateNumberSuccessfully_WhenNewValueIsValid(
        Order sut,
        string newOrderNumber)
    {
        // Arrange
        var orderItem = new OrderItem(sut.Id, $"not{newOrderNumber}", 1, "шт.");
        sut.AddOrderItem(orderItem);

        // Act
        var act = () => sut.UpdateNumber(newOrderNumber);

        // Assert
        act.Should().NotThrow();
        sut.Number.Should().Be(newOrderNumber);
    }
    
    
    [Theory, AutoData]
    public void OrderItemUpdateName_ThrowDomainException_WhenNewOrderNumberEqualToOneOfOrderItemsNames(
        OrderItem sut,
        string orderName)
    {
        // Arrange
        var newItemName = orderName;
        
        // Act
        var act = () => sut.UpdateName(newItemName, orderName);

        // Assert
        act.Should().Throw<DomainException>();
    }


    [Theory, AutoData]
    public void OrderItemUpdateName_UpdateNameSuccessfully_WhenNewValueIsValid(
        OrderItem sut,
        string orderName)
    {
        // Arrange
        var newItemName = $"not{orderName}";
        
        // Act
        var act = () => sut.UpdateName(newItemName, orderName);

        // Assert
        act.Should().NotThrow();
        sut.Name.Should().Be(newItemName);
    }
}