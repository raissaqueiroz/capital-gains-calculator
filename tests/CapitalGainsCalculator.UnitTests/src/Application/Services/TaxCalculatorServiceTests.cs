using System;
using System.Linq;
using AutoFixture;
using CapitalGainsCalculator.Application.services;
using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Enums;
using CapitalGainsCalculator.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace CapitalGainsCalculator.UnitTests.Application.Services;

public class TaxCalculatorServiceTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly Mock<ITaxPolicy> _taxPolicyMock = new(MockBehavior.Strict);
    private readonly Mock<ILossTrackerService> _lossTrackerServiceMock = new(MockBehavior.Strict);
    
    private Operation CreateOperation(OperationType operationType, int quantity, decimal unitCost)
    {
        return _fixture
            .Build<Operation>()
            .With(entity => entity.Type, operationType)
            .With(entity => entity.Quantity, quantity)
            .With(entity => entity.UnitCost, unitCost)
            .Create();
    }

    [Fact]
    public void GivenOperation_WhenOperationTypeIsBuy_ThenShouldReturnsZeroTax()
    {
        var operation = CreateOperation(OperationType.Buy, 10, 10m);

        var result = GetService().CalculateTaxes(new[] { operation });
        
        result.Should().ContainSingle().Which.Tax.Should().Be(0);
        _taxPolicyMock.Verify(service => service.IsTaxExempt(operation), Times.Never);
        _taxPolicyMock.Verify(service => service.CalculateTax(It.IsAny<decimal>(), operation), Times.Never);
        _lossTrackerServiceMock.Verify(service => service.Accumulate(It.IsAny<decimal>()), Times.Never);
        _lossTrackerServiceMock.Verify(service => service.ApplyToLoss(It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void GivenSellWithLoss_WhenCalculateTax_ThenItShouldAccumulatedLossAndTaxIsZero()
    {
        var buyOperation = CreateOperation(OperationType.Buy, 10, 10m);
        var sellOperation = CreateOperation(OperationType.Sell, 10, 8m);
        
        _lossTrackerServiceMock.Setup(service => service.Accumulate(20)).Verifiable();
        
        var result = GetService().CalculateTaxes(new[] { buyOperation, sellOperation });
        
        result.Last().Tax.Should().Be(0);
        
        _lossTrackerServiceMock.Verify(service => service.Accumulate(20), Times.Once);
    }

    [Fact]
    public void GivenSellWithProfit_AndIsExempt_ThenNoLossAppliedAndTaxIsZero()
    {
        var buyOperation = CreateOperation(OperationType.Buy, 100, 10m);
        var sellOperation = CreateOperation(OperationType.Sell, 100, 15m);
        
        _taxPolicyMock.Setup(service => service.CalculateTax(500, sellOperation)).Returns(0).Verifiable();
        
        _taxPolicyMock.Setup(service => service.IsTaxExempt(sellOperation)).Returns(true);
        
        var result = GetService().CalculateTaxes(new[] { buyOperation, sellOperation });

        result.Should().HaveCount(2);
        result.Last().Tax.Should().Be(0);
        
        _taxPolicyMock.Verify(service => service.CalculateTax(500, sellOperation), Times.Once);
        _taxPolicyMock.Verify(service => service.IsTaxExempt(sellOperation), Times.Once);
        _lossTrackerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GivenSellWithProfit_AndLossAvailable_ThenLossIsAppliedBeforeTax()
    {
        var buyOperation = CreateOperation(OperationType.Buy, 20, 10m);
        var sellOperationWithLoss = CreateOperation(OperationType.Sell, 10, 7);
        var sellOperationWithProfit = CreateOperation(OperationType.Sell, 10, 15m);
        decimal accumulatedLoss = 0;
        
        _lossTrackerServiceMock
            .Setup(service => service.Accumulate(30))
            .Callback<decimal>(value => accumulatedLoss +=value)
            .Verifiable();
        
        _lossTrackerServiceMock
            .Setup(service => service.ApplyToLoss(50))
            .Returns<decimal>(value =>
            {
                var result = Math.Max(0, value - accumulatedLoss);
                accumulatedLoss = Math.Max(0, accumulatedLoss - value);
                return result;
            })
            .Verifiable();
        
        _lossTrackerServiceMock
            .Setup(service => service.AccumulatedLoss)
            .Returns(() => accumulatedLoss)
            .Verifiable();
        
        _taxPolicyMock
            .Setup(service => service.IsTaxExempt(sellOperationWithProfit))
            .Returns(false)
            .Verifiable();
        
        
        _taxPolicyMock
            .Setup(service => service.CalculateTax(20, sellOperationWithProfit))
            .Returns(4)
            .Verifiable();
        
        var result = GetService().CalculateTaxes(new[] { buyOperation, sellOperationWithLoss, sellOperationWithProfit }).ToList();

        result.Should().HaveCount(3);
        result[0].Tax.Should().Be(0);
        result[1].Tax.Should().Be(0);
        result[2].Tax.Should().Be(4);
        
        _lossTrackerServiceMock.Verify(service => service.Accumulate(30), Times.Once);
        _taxPolicyMock.Verify(service => service.IsTaxExempt(sellOperationWithProfit), Times.Once);
        _lossTrackerServiceMock.Verify(service => service.ApplyToLoss(50), Times.Once);
        _taxPolicyMock.Verify(service => service.CalculateTax(20, sellOperationWithProfit), Times.Once);
    }

    [Fact]
    public void GivenSellWithProfit_AndNoLossAvailable_ThenFullTaxIsApplied()
    {
        var buyOperation = CreateOperation(OperationType.Buy, 2000, 10m);
        var sellOperation = CreateOperation(OperationType.Sell, 2000, 11m);
        
        _lossTrackerServiceMock
            .Setup(service => service.AccumulatedLoss)
            .Returns(0)
            .Verifiable();
        
        _taxPolicyMock
            .Setup(service => service.IsTaxExempt(sellOperation))
            .Returns(false)
            .Verifiable();
        
        _taxPolicyMock
            .Setup(service => service.CalculateTax(2000, sellOperation))
            .Returns(400)
            .Verifiable();
        
        var result = GetService().CalculateTaxes(new[] { buyOperation, sellOperation }).ToList();
        
        result.Should().HaveCount(2);
        result[0].Tax.Should().Be(0);
        result[1].Tax.Should().Be(400);
        
        _taxPolicyMock.Verify(service => service.IsTaxExempt(sellOperation), Times.Once);
        _taxPolicyMock.Verify(service => service.CalculateTax(2000, sellOperation), Times.Once);
    }
    
    public TaxCalculatorService GetService() => new TaxCalculatorService(
        _taxPolicyMock.Object, _lossTrackerServiceMock.Object);
}