using CapitalGainsCalculator.Domain.Entities;

namespace CapitalGainsCalculator.Application.services;

public class DefaultTaxPolicy : ITaxPolicy
{
    private const decimal TaxRate = 0.20m;
    private const decimal ExemptionThreshold = 20000m;
    
    public decimal CalculateTax(decimal amountDifference, Operation operation)
    {
        if (IsTaxExempt(operation))
            return 0;
        
        return amountDifference * TaxRate;
    }

    public bool IsTaxExempt(Operation operation) => operation is { IsSell: true, TotalValue: <= ExemptionThreshold };
}