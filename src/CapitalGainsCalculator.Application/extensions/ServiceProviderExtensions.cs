using CapitalGainsCalculator.Application.factories;
using CapitalGainsCalculator.Application.services;
using Microsoft.Extensions.DependencyInjection;

namespace CapitalGainsCalculator.Application.extensions;

public static class ServiceProviderExtensions
{
    public static ITaxCalculatorService CreateTaxCalculatorService(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<ITaxCalculatorFactory>().Create();
}