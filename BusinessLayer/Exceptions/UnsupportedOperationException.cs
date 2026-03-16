namespace BusinessLayer.Exceptions;

public class UnsupportedOperationException : QuantityMeasurementException
{
    public string Operation      { get; }
    public string MeasurementType { get; }

    public UnsupportedOperationException(string operation, string measurementType)
        : base($"{operation} is not supported for {measurementType} measurements.", "UNSUPPORTED_OP")
    {
        Operation       = operation;
        MeasurementType = measurementType;
    }
}
