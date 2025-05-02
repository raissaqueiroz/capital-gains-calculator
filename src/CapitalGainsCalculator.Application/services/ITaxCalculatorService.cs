using CapitalGainsCalculator.Domain.Entities;

namespace CapitalGainsCalculator.Application.services;

public interface ITaxCalculatorService
{
    IEnumerable<TaxResult> CalculateTaxes(IEnumerable<Operation> operations);
}