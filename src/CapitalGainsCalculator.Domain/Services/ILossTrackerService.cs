namespace CapitalGainsCalculator.Domain.Services;

public interface ILossTrackerService
{
    decimal AccumulatedLoss { get; } 
    
    void Accumulate(decimal loss);
    decimal ApplyToLoss(decimal amountDifference);
}