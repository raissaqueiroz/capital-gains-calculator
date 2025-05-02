using CapitalGainsCalculator.Application.extensions;
using CapitalGainsCalculator.Application.factories;
using CapitalGainsCalculator.Application.services;
using CapitalGainsCalculator.CLI.Utils;
using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace CapitalGainsCalculator.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt")
            .CreateLogger();
        
        ConsoleUI.Welcome();

        string inputLine;
        var results = new List<IList<TaxResult>>();
        
        while (!string.IsNullOrEmpty(inputLine = Console.ReadLine()))
        {
            try
            {
                var taxCalculatorService = host.Services.CreateTaxCalculatorService();
                var operations = JsonConvert.DeserializeObject<IEnumerable<Operation>>(inputLine);
                var taxResults = taxCalculatorService.CalculateTaxes(operations);
                
                results.Add(taxResults.ToList());
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured: {ex.Message}");
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
    }

    static ITaxCalculatorService GetTaxCalculatorService(IHost host)
    {
        return host.Services
            .GetRequiredService<ITaxCalculatorFactory>()
            .Create();
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<ITaxPolicy, DefaultTaxPolicy>();
                services.AddScoped<ILossTrackerService, LossTrackerService>();
                services.AddScoped<ITaxCalculatorService, TaxCalculatorService>();
                services.AddScoped<ITaxCalculatorFactory, TaxCalculatorFactory>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });
}