using System.Collections.Generic;
using AutoFixture;
using CapitalGainsCalculator.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CapitalGainsCalculator.UnitTests.Domain.ValueObjects;

public class StockPositionTests
{
    public static IEnumerable<object[]> StockPositionTestDataBuyScenarios =>
        new List<object[]>
        {
            new object[] { "same unit cost", 100, 10m, 100, 10m, 200, 10m },
            new object[] { "different cost increases avg", 100, 10m, 100, 20m, 200, 15m },
            new object[] { "different cost decreases avg", 100, 20m, 100, 10m, 200, 15m },
        };
    
    public static IEnumerable<object[]> StockPositionTestDataSellScenarios =>
        new List<object[]>
        {
            new object[] { "profit", 10, 5, 5.0m, 10.0m, 25.0m },
            new object[] { "no profit", 10, 5, 10.0m, 10.0m, 0.0m },
            new object[] { "loss", 10, 5, 12.0m, 10.0m, -10.0m },
        };

    [Fact]
    public void GivenInitialPosition_WhenBuy_ThenQuantityAndAvarageShouldUpdated()
    {
        // Arrange
        var position = new StockPosition();
        var quantity = 100;
        var unitCost = 10.0m;
        
        // Act
        position.Buy(quantity, unitCost);
        
        // Assert
        position.Quantity.Should().Be(quantity);
        position.AveragePrice.Should().Be(unitCost);
    }

    [Theory]
    [MemberData(nameof(StockPositionTestDataBuyScenarios))]
    public void GivenExistingPosition_WhenBuy_ThenAvarageAndQuantityShouldRecalculateCorrectly(
        string scenario,
        int initialQuantity,
        decimal initialPrice,
        int newQuantity,
        decimal newPrice,
        int expectedQuantityTotal,
        decimal expectedAveragePrice
    )
    {
        // Arrange
        var position = new StockPosition();
        position.Buy(initialQuantity, initialPrice);
        
        // Act
        position.Buy(newQuantity, newPrice);
        
        // Assert
        position.Quantity.Should().Be(expectedQuantityTotal, because: $"Scenario: {scenario}.");
        position.AveragePrice.Should().Be(expectedAveragePrice, because: $"Scenario: {scenario}.");
    }

    [Fact]
    public void GivenPositionWithQuantity_WhenSell_ThenQuantityShouldDecrease()
    {
        // Arrange
        var position = new StockPosition();
        position.Buy(100, 10m);
        
        // Act
        position.Sell(40);
        
        // Assert
        position.Quantity.Should().Be(60);
    }
    
    [Theory]
    [MemberData(nameof(StockPositionTestDataSellScenarios))]
    public void GivenSellOperation_WhenCalculateProfit_ThenReturnsCorrectValue(
        string scenario,
        int buyQuantity,
        int sellQuantity,
        decimal buyPrice,
        decimal sellPrice,
        decimal expectedDifferentAmount
    )
    {
        // Arrange
        var position = new StockPosition();
        position.Buy(buyQuantity, buyPrice);
        
        // Act
        var profit = position.CalculateDiffAmount(sellPrice, sellQuantity);
        
        // Assert
        profit.Should().Be(expectedDifferentAmount, because: $"Scenario: {scenario}.");
    }
}