using ModelLayer.Entities;

namespace ModelLayer.DTOs;

/// <summary>
/// Response DTO for the REST API layer.
/// Maps to/from QuantityMeasurementEntity for clean API communication.
/// </summary>
public class QuantityMeasurementDTO
{
    public int     Id              { get; set; }
    public string  MeasurementType { get; set; } = string.Empty;
    public string  OperationType   { get; set; } = string.Empty;
    public double  Value1          { get; set; }
    public double  Value2          { get; set; }
    public double  Result          { get; set; }
    public string  Unit            { get; set; } = string.Empty;
    public DateTime CreatedAt      { get; set; }
    public bool    IsError         { get; set; }
    public string? ErrorMessage    { get; set; }

    public QuantityMeasurementDTO() { }

    public QuantityMeasurementDTO(string measurementType, string operationType,
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
        IsError         = false;
    }

    public QuantityMeasurementDTO(string measurementType, string operationType,
                                  double value1, double value2, string errorMessage)
    {
        MeasurementType = measurementType;
        OperationType   = operationType;
        Value1          = value1;
        Value2          = value2;
        IsError         = true;
        ErrorMessage    = errorMessage;
        CreatedAt       = DateTime.UtcNow;
    }

    // ─── Static Factory Methods ───────────────────────────────────────────────

    /// <summary>Converts a QuantityMeasurementEntity to a DTO.</summary>
    public static QuantityMeasurementDTO FromEntity(QuantityMeasurementEntity entity)
    {
        return new QuantityMeasurementDTO
        {
            Id              = entity.Id,
            MeasurementType = entity.MeasurementType,
            OperationType   = entity.OperationType,
            Value1          = entity.Value1,
            Value2          = entity.Value2,
            Result          = entity.Result,
            Unit            = entity.Unit,
            CreatedAt       = entity.CreatedAt,
            IsError         = entity.HasError,
            ErrorMessage    = entity.ErrorMessage
        };
    }

    /// <summary>Converts a list of entities to a list of DTOs using LINQ.</summary>
    public static List<QuantityMeasurementDTO> FromEntityList(List<QuantityMeasurementEntity> entities)
        => entities.Select(FromEntity).ToList();

    /// <summary>Converts this DTO back to a QuantityMeasurementEntity.</summary>
    public QuantityMeasurementEntity ToEntity()
    {
        return new QuantityMeasurementEntity
        {
            Id              = this.Id,
            MeasurementType = this.MeasurementType,
            OperationType   = this.OperationType,
            Value1          = this.Value1,
            Value2          = this.Value2,
            Result          = this.Result,
            Unit            = this.Unit,
            CreatedAt       = this.CreatedAt,
            HasError        = this.IsError,
            ErrorMessage    = this.ErrorMessage
        };
    }
}
