namespace CapitalGainsCalculator.Domain.Services;

public class LossTrackerService : ILossTrackerService
{
    public decimal AccumulatedLoss { get; private set; }

    public void Accumulate(decimal loss)
    {
        AccumulatedLoss += loss;
    }

    public decimal ApplyToLoss(decimal amountDifference)
    {
        var amountDifferenceAfterLossApplied = Math.Max(0, amountDifference - AccumulatedLoss);
        AccumulatedLoss = Math.Max(0, AccumulatedLoss - amountDifference);
        return amountDifferenceAfterLossApplied;
    }
}