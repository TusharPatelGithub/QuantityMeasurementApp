using ModelLayer.Models;

namespace ModelLayer.Entities;

public class QuantityMeasurementEntity
{
    public int Id { get; set; }
    public string MeasurementType { get; set; } = string.Empty;
    public string OperationType { get; set; }   = string.Empty;
    public double Value1 { get; set; }
    public double Value2 { get; set; }
    public double Result { get; set; }
    public string Unit { get; set; }            = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }

    public QuantityMeasurementEntity() { }

    public QuantityMeasurementEntity(string measurementType, string operationType,
                                     double value1, double value2,
                                     double result, string unit)
    {
        MeasurementType = measurementType;
        OperationType   = operationType;
        Value1          = value1;
        Value2          = value2;
        Result          = result;
        Unit            = unit;
        CreatedAt       = DateTime.UtcNow;
        HasError        = false;
    }

    public QuantityMeasurementEntity(string measurementType, string operationType,
                                     double value1, double value2, string errorMessage)
    {
        MeasurementType = measurementType;
        OperationType   = operationType;
        Value1          = value1;
        Value2          = value2;
        ErrorMessage    = errorMessage;
        CreatedAt       = DateTime.UtcNow;
        HasError        = true;
    }
}
