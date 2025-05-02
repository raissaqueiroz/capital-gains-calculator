using CapitalGainsCalculator.Domain.Entities;

namespace CapitalGainsCalculator.Application.services;

public interface ITaxPolicy
{
    decimal CalculateTax(decimal amountDifference, Operation operation);
    bool IsTaxExempt(Operation operation);
}