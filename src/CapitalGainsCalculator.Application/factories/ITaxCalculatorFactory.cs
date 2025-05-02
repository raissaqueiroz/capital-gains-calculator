using CapitalGainsCalculator.Application.services;

namespace CapitalGainsCalculator.Application.factories;

public interface ITaxCalculatorFactory
{
    ITaxCalculatorService Create();
}