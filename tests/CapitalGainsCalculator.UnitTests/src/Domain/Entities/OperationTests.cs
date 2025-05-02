using System.Collections.Generic;
using AutoFixture;
using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Enums;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CapitalGainsCalculator.UnitTests.Domain.Entities;

public class OperationTests
{
    private readonly IFixture _fixture = new Fixture();

    public static IEnumerable<object[]> OperationsTestData =>
        new List<object[]>
        {
            new object[]
            {
                "[{\"operation\":\"buy\", \"unit-cost\":10.00, \"quantity\":10000}, {\"operation\":\"sell\", \"unit-cost\":20.00, \"quantity\":5000}]",
                new[]
                {
                    new Operation { Type = OperationType.Buy, UnitCost = 10.00m, Quantity = 10000 },
                    new Operation { Type = OperationType.Sell, UnitCost = 20.00m, Quantity = 5000 },
                }
            },
            new object[]
            {
                "[{\"operation\":\"buy\", \"unit-cost\":20.00, \"quantity\":10000}, {\"operation\":\"sell\", \"unit-cost\":10.00, \"quantity\":5000}]",
                new[]
                {
                    new Operation { Type = OperationType.Buy, UnitCost = 20.00m, Quantity = 10000 },
                    new Operation { Type = OperationType.Sell, UnitCost = 10.00m, Quantity = 5000 },
                }
            }
        };
    
    [Theory]
    [InlineData(10, 5.5, 55.0)]
    [InlineData(0, 10.0, 0.0)]
    [InlineData(3, 0.0, 0.0)]
    public void GivenOperation_WhenGettingTotalValue_ThenShouldReturnCorrectResult(int quantity, decimal unitCost, decimal expectedResult)
    {
        //Arrange 
        var operation = _fixture
            .Build<Operation>()
            .With(entity => entity.Quantity, quantity)
            .With(entity => entity.UnitCost, unitCost)
            .Create();
            
        //Act & Assert
        operation.TotalValue.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(OperationType.Buy, true, false)]
    [InlineData(OperationType.Sell, false, true)]
    public void GivenOperation_WhenCheckingType_ThenShouldMatchExpected(OperationType operationType, bool isBuyExpected,
        bool isSellExpected)
    {
        //Arrange 
        var operation = _fixture
            .Build<Operation>()
            .With(entity => entity.Type, operationType)
            .Create();
        
        // Act & Assert
        operation.IsBuy.Should().Be(isBuyExpected);
        operation.IsSell.Should().Be(isSellExpected);
    }

    [Theory]
    [MemberData(nameof(OperationsTestData))]
    public void GivenOperation_WhenDeserialized_ThenValuesShouldBePreserved(string json, Operation[] operationExpected)
    {
        // Act
        var result = JsonConvert.DeserializeObject<IEnumerable<Operation>>(json);
        
        // Assert
        result.Should().BeEquivalentTo(operationExpected);
    }
}