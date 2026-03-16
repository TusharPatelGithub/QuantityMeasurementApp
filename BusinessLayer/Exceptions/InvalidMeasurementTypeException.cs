namespace BusinessLayer.Exceptions;

public class InvalidMeasurementTypeException : QuantityMeasurementException
{
    public string MeasurementType { get; }

    public InvalidMeasurementTypeException(string measurementType)
        : base($"Unsupported measurement type: '{measurementType}'", "INVALID_TYPE")
    {
        MeasurementType = measurementType;
    }
}
