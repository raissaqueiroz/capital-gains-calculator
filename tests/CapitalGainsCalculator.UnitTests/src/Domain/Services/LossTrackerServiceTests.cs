using System.Collections.Generic;
using AutoFixture;
using CapitalGainsCalculator.Domain.Services;
using FluentAssertions;
using Xunit;

namespace CapitalGainsCalculator.UnitTests.Domain.Services;

public class LossTrackerServiceTests
{
    private readonly IFixture _fixture = new Fixture();

    public static IEnumerable<object[]> LossTestData => 
        new List<object[]>
        {
            new object[] { "no loss", 100m, 0m, 100m },
            new object[] { "partial loss applied", 100m, 50m, 50m },
            new object[] { "full loss applied", 100m, 100m, 0m },
            new object[] { "loss fully covers, extra ignored", 100m, 150m, 0m }
        };
    
    [Fact]
    public void GivenLoss_WhenAccumulate_ThenItShoulBeAddedToAccumulatedLoss()
    {
        // Arrange
        var service = new LossTrackerService();
        var loss = _fixture.Create<decimal>();
        
        // Act
        service.Accumulate(loss);
        
        // Assert
        service.AccumulatedLoss.Should().Be(loss);
    }

    [Fact]
    public void GivenMultipleLosses_WhenAccumulate_ThenItShouldSumThemAll()
    {
        // Arrange
        var service = new LossTrackerService();
        
        var lossOne = _fixture.Create<decimal>();
        var lossTwo = _fixture.Create<decimal>();
        var lossThree = _fixture.Create<decimal>();
        var expectedResult = lossOne + lossTwo + lossThree;
        
        // Act
        service.Accumulate(lossOne);
        service.Accumulate(lossTwo);
        service.Accumulate(lossThree);
        
        // Assert
        service.AccumulatedLoss.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(LossTestData))]
    public void GivenAccumulatedLoss_WhenApplyToLoss_ThenTaxableProfitShouldBeReduced(string scenario, decimal amountDifference,
        decimal accumulatedLoss, decimal expectedTaxable)
    {
        // Arrange
        var service = new LossTrackerService();
        service.Accumulate(accumulatedLoss);
        
        // Act
        var taxable = service.ApplyToLoss(amountDifference);
        
        // Assert
        taxable.Should().Be(expectedTaxable, because: $"Scenario: {scenario}");
    }

    [Theory]
    [InlineData(100, 50, 0)]
    [InlineData(20, 33, 13)]
    public void GivenApplyToLoss_WhenCalled_ThenAccumulatedLossShouldBeUpdatedCorrectly(decimal amountDifference,
        decimal initialLoss, decimal expectedRemainingLoss)
    {
        // Arrange
        var service = new LossTrackerService();
        service.Accumulate(initialLoss);
        
        // Act
        service.ApplyToLoss(amountDifference);
        
        // Assert
        service.AccumulatedLoss.Should().Be(expectedRemainingLoss);
    }
}