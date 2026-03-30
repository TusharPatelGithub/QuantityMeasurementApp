using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

public class QuantityDTO
{
    [Required(ErrorMessage = "Value is required.")]
    public double Value { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public string Unit { get; set; }

    [Required(ErrorMessage = "MeasurementType is required.")]
    public string MeasurementType { get; set; }

    public QuantityDTO(double value, string unit, string measurementType)
    {
        Value           = value;
        Unit            = unit;
        MeasurementType = measurementType;
    }

    // Parameterless constructor required for model binding
    public QuantityDTO()
    {
        Unit            = string.Empty;
        MeasurementType = string.Empty;
    }
}

