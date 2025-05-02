namespace CapitalGainsCalculator.Domain.Entities;

public class TaxResult
{
    private decimal _tax;
    public decimal Tax { get => Math.Round(_tax, 1); set => _tax = value; }
}