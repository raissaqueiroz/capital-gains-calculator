using CapitalGainsCalculator.Application.services;
using Microsoft.Extensions.DependencyInjection;

namespace CapitalGainsCalculator.Application.factories;

public class TaxCalculatorFactory(IServiceProvider serviceProvider) : ITaxCalculatorFactory
{
    public ITaxCalculatorService Create()
    {
        return serviceProvider.GetRequiredService<ITaxCalculatorService>();
    }
}