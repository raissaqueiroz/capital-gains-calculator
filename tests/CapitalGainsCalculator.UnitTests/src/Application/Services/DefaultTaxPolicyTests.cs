using System.Collections.Generic;
using AutoFixture;
using CapitalGainsCalculator.Application.services;
using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CapitalGainsCalculator.UnitTests.Application.Services;

public class DefaultTaxPolicyTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly ITaxPolicy _taxPolicy = new DefaultTaxPolicy();
    
    private Operation CreateOperation(OperationType operationType, int quantity, decimal unitCost)
    {
        return _fixture
            .Build<Operation>()
            .With(entity => entity.Type, operationType)
            .With(entity => entity.Quantity, quantity)
            .With(entity => entity.UnitCost, unitCost)
            .Create();
    }
    
    public static System.Collections.Generic.IEnumerable<object[]> CalculateTaxTestData => 
        new List<object[]>
        {
            new object[] { "Exempt sell under threshold", OperationType.Sell, 10, 1500m, 100m, 0m},
            new object[] { "Exempt sell at threshold", OperationType.Sell, 10, 2000m, 100m, 0m },
            new object[] { "Taxed sell over threshold", OperationType.Sell, 10, 2500m, 100m, 20m },
            new object[] { "Buy operation is always taxed", OperationType.Buy, 10, 2500m, 100m, 20m }
        };
    
    public static System.Collections.Generic.IEnumerable<object[]> IsTaxExemptTestData => 
        new List<object[]>
        {
            new object[] { "Exempt: sell under threshold", OperationType.Sell, 10, 1500m, true},
            new object[] { "Not Exempt: sell over threshold", OperationType.Sell, 10, 2500m, false },
            new object[] { "Not Exempt: buy operation", OperationType.Buy, 10, 1500m, false }
        };

    [Theory]
    [MemberData(nameof(CalculateTaxTestData))]
    public void GivenOperation_WhenCalculateTax_ThenReturnsExpectedTax(
        string scenario,
        OperationType type,
        int quantity,
        decimal unitCost,
        decimal amountDifference,
        decimal expectedTax
    )
    {
        // Arrange
        var operation = CreateOperation(type, quantity, unitCost);
        
        // Act
        var tax = _taxPolicy.CalculateTax(amountDifference, operation);
        
        // Assert
        tax.Should().Be(expectedTax, because: $"Scenario {scenario}");
    }
    
    [Theory]
    [MemberData(nameof(IsTaxExemptTestData))]
    public void GivenOperation_WhenCheckingExemption_ThenReturnsCorrectValue(
        string scenario,
        OperationType type,
        int quantity,
        decimal unitCost,
        bool expectedExemption
    )
    {
        // Arrange
        var operation = CreateOperation(type, quantity, unitCost);
        
        // Act
        var isTaxExempt = _taxPolicy.IsTaxExempt(operation);
        
        // Assert
        isTaxExempt.Should().Be(expectedExemption, because: $"Scenario {scenario}");
    }
    
}