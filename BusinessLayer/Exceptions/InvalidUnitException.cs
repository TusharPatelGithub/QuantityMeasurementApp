namespace BusinessLayer.Exceptions;

public class InvalidUnitException : QuantityMeasurementException
{
    public string Unit { get; }
    public string MeasurementType { get; }

    public InvalidUnitException(string unit, string measurementType)
        : base($"Unknown {measurementType} unit: '{unit}'", "INVALID_UNIT")
    {
        Unit            = unit;
        MeasurementType = measurementType;
    }
}
