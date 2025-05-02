namespace CapitalGainsCalculator.Domain.ValueObjects;

public class StockPosition
{
    public int Quantity { get; private set; } = 0;
    public decimal AveragePrice { get; private set; } = 0;

    public void Buy(int quantity, decimal unitCost)
    {
        AveragePrice = ((Quantity * AveragePrice) + (quantity * unitCost)) / (Quantity + quantity);
        Quantity += quantity;
    }
    
    public void Sell(int quantity) => Quantity -= quantity;

    public decimal CalculateDiffAmount(decimal sellUnitCost, int sellQuantity) => (sellUnitCost-AveragePrice)*sellQuantity;
}