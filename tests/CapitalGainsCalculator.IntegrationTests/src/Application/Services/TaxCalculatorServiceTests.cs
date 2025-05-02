using System.Collections.Generic;
using CapitalGainsCalculator.Application.services;
using CapitalGainsCalculator.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CapitalGainsCalculator.IntegrationTests.Application.Services;

public class TaxCalculatorServiceTests(TaxCalculatorFixture fixture) : IClassFixture<TaxCalculatorFixture>
{


    [Theory]
    [MemberData(nameof(TaxCalculatorFixture.GetTestCases), MemberType = typeof(TaxCalculatorFixture))]
    public void Should_Calculate_Tax_Correctly(TestCaseData.TaxTestCaseData testCaseData)
    {
        var result = fixture.GetTaxCalculatorService().CalculateTaxes(testCaseData.Operations);
        result.Should().BeEquivalentTo(testCaseData.ExpectedResults);
    }
    
}