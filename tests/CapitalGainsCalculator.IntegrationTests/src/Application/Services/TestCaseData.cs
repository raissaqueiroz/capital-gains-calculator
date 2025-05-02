using System.Collections.Generic;
using AutoFixture;
using CapitalGainsCalculator.Domain.Entities;
using CapitalGainsCalculator.Domain.Enums;

namespace CapitalGainsCalculator.IntegrationTests.Application.Services;

public static class TestCaseData
{
    public static TaxTestCaseData Case1() => new()
    {
        CaseName = "Case 1: Venda com Lucro Abaixo de 20k (Isento)",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 100, 10.00m),
            CreateOperation(OperationType.Sell, 50, 15.00m),
            CreateOperation(OperationType.Sell, 50, 15.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new() { Tax = 0.0m },
            new() { Tax = 0.0m },
            new() { Tax = 0.0m },
        }
    };
    
    public static TaxTestCaseData Case2() => new()
    { 
        CaseName = "Case 2: Venda com Lucro Acima de 20k (Tributado)",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Sell, 5000, 20.00m),
            CreateOperation(OperationType.Sell, 5000, 5.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 10000.0m },
            new TaxResult() { Tax = 0.0m },
        }
    };
    
    public static TaxTestCaseData Case3() => new()
    {
        CaseName = "Case 3: Prejuízo Seguido de Lucro (Compensação Parcial)",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Sell, 5000, 5.00m),
            CreateOperation(OperationType.Sell, 3000, 20.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 1000.0m },
        }
    };
    
    public static TaxTestCaseData Case4() => new()
    {
        CaseName = "Case 4: Venda com Prejuízo, Nenhum Imposto Devido",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Buy, 5000, 25.00m),
            CreateOperation(OperationType.Sell, 10000, 15.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
        }
    };
    
    public static TaxTestCaseData Case5() => new()
    {
        CaseName = "Case 5: Prejuízo e Lucro Subsequente Acima de 20K",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Buy, 5000, 25.00m),
            CreateOperation(OperationType.Sell, 10000, 15.00m),
            CreateOperation(OperationType.Sell, 5000, 25.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 10000.0m },
        }
    };
    
    public static TaxTestCaseData Case6() => new()
    {
        CaseName = "Case 6: Lucros Acumulados com Compensação de Prejuízo",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Sell, 5000, 2.00m),
            CreateOperation(OperationType.Sell, 2000, 20.00m),
            CreateOperation(OperationType.Sell, 2000, 20.00m),
            CreateOperation(OperationType.Sell, 1000, 25.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 3000.0m },
        }
    };
    
    public static TaxTestCaseData Case7() => new()
    {
        CaseName = "Case 7: Compra Adicional Após Prejuízo e Venda com Lucro",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Sell, 5000, 2.00m),
            CreateOperation(OperationType.Sell, 2000, 20.00m),
            CreateOperation(OperationType.Sell, 2000, 20.00m),
            CreateOperation(OperationType.Sell, 1000, 25.00m),
            CreateOperation(OperationType.Buy, 10000, 20.00m),
            CreateOperation(OperationType.Sell, 5000, 15.00m),
            CreateOperation(OperationType.Sell, 4350, 30.00m),
            CreateOperation(OperationType.Sell, 650, 30.00m),
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 3000.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 3700.0m },
            new TaxResult() { Tax = 0.0m },
        }
    };
    
    public static TaxTestCaseData Case8() => new()
    {
        CaseName = "Case 8: Vendas Sucessivas Acima de 20k com Lucro",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10000, 10.00m),
            CreateOperation(OperationType.Sell, 10000, 50.00m),
            CreateOperation(OperationType.Buy, 10000, 20.00m),
            CreateOperation(OperationType.Sell, 10000, 50.00m)
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 80000.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 60000.0m },
        }
    };
    
    public static TaxTestCaseData Case9() => new()
    {
        CaseName = "Case 9: Operações Complexas com Multiplas Compras e Vendas",
        Operations = new List<Operation>
        {
            CreateOperation(OperationType.Buy, 10, 5000.00m),
            CreateOperation(OperationType.Sell, 5, 4000.00m),
            CreateOperation(OperationType.Buy, 5, 15000.00m),
            CreateOperation(OperationType.Buy, 2, 4000.00m),
            CreateOperation(OperationType.Buy, 2, 23000.00m),
            CreateOperation(OperationType.Sell, 1, 20000.00m),
            CreateOperation(OperationType.Sell, 10, 12000.00m),
            CreateOperation(OperationType.Sell, 3, 15000.00m),
        },
        ExpectedResults = new List<TaxResult>
        {
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 0.0m },
            new TaxResult() { Tax = 1000.0m },
            new TaxResult() { Tax = 2400.0m },
        }
    };
    
    private static Operation CreateOperation(OperationType operationType, int quantity, decimal unitCost)
    {
        var fixture = new Fixture();
        
        return fixture
            .Build<Operation>()
            .With(entity => entity.Type, operationType)
            .With(entity => entity.Quantity, quantity)
            .With(entity => entity.UnitCost, unitCost)
            .Create();
    }

    public class TaxTestCaseData
    {
        public string CaseName { get; init; } = "";
        public IList<Operation> Operations { get; init; } = new List<Operation>();
        public IList<TaxResult> ExpectedResults { get; init; } = new List<TaxResult>();
        
    }
}