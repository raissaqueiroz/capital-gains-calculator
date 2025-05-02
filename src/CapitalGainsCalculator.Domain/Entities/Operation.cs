using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CapitalGainsCalculator.Domain.Enums;

namespace CapitalGainsCalculator.Domain.Entities;

public class Operation
{
    [JsonProperty("operation")]
    [JsonConverter(typeof(StringEnumConverter))]
    public OperationType Type { get; set; }
    [JsonProperty("unit-cost")]
    public decimal UnitCost { get; set; }
    [JsonProperty("quantity")]
    public int Quantity { get; set; }
    [JsonIgnore]
    public decimal TotalValue => Quantity*UnitCost; 
    [JsonIgnore]
    public bool IsBuy => Type == OperationType.Buy;
    [JsonIgnore]
    public bool IsSell => Type == OperationType.Sell;
}