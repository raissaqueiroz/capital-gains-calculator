using System.Collections.Generic;
using CapitalGainsCalculator.Application.services;
using CapitalGainsCalculator.Domain.Services;

namespace CapitalGainsCalculator.IntegrationTests.Application.Services;

public class TaxCalculatorFixture
{
    public ITaxCalculatorService GetTaxCalculatorService() =>
        new TaxCalculatorService(new DefaultTaxPolicy(), new LossTrackerService());
    public static IEnumerable<object[]> GetTestCases()
    {
        yield return [TestCaseData.Case1()];
        yield return [TestCaseData.Case2()];
        yield return [TestCaseData.Case3()];
        yield return [TestCaseData.Case4()];
        yield return [TestCaseData.Case5()];
        yield return [TestCaseData.Case6()];
        yield return [TestCaseData.Case7()];
        yield return [TestCaseData.Case8()];
        yield return [TestCaseData.Case9()];
    }
}