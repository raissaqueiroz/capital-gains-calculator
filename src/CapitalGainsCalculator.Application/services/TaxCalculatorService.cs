using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Services;
using CapitalGainsCalculator.Domain.ValueObjects;

namespace CapitalGainsCalculator.Application.services;

public class TaxCalculatorService(ITaxPolicy taxPolicy, ILossTrackerService lossTrackerService)
    : ITaxCalculatorService
{
    private readonly StockPosition _stockPosition = new ();
    
    public IEnumerable<TaxResult> CalculateTaxes(IEnumerable<Operation> operations)
    {
        IList<TaxResult> taxResults = new List<TaxResult>();
        foreach (var operation in operations)
        {
            if (operation.IsBuy)
            {
                HandleBuy(operation);
                taxResults.Add(new TaxResult { Tax = 0 });
            }
            
            if (operation.IsSell)
            {
                var taxResult = HandleSell(operation);
                taxResults.Add(taxResult);
            }
        }
        
        return taxResults;
    }

    private void HandleBuy(Operation operation) => _stockPosition.Buy(operation.Quantity, operation.UnitCost);
    
    private TaxResult HandleSell(Operation operation) 
    {
        var amountDifference = _stockPosition.CalculateDiffAmount(operation.UnitCost, operation.Quantity);
        
        if (amountDifference < 0)
        {
            lossTrackerService.Accumulate(Math.Abs(amountDifference));
            _stockPosition.Sell(operation.Quantity);
            return new TaxResult { Tax = 0 };
        }

        if (ShouldApplyLoss(amountDifference, operation))
        {
            amountDifference = lossTrackerService.ApplyToLoss(amountDifference);
        }
        
        var tax = taxPolicy.CalculateTax(amountDifference, operation);
        _stockPosition.Sell(operation.Quantity);
        return new TaxResult { Tax = tax };
    }

    private bool ShouldApplyLoss(decimal amountDifference, Operation operation) =>
        !taxPolicy.IsTaxExempt(operation) && HasAccumulatedLoss();
    
    private bool HasAccumulatedLoss() => lossTrackerService.AccumulatedLoss > 0;
}