namespace BusinessLayer.Exceptions;

public class MeasurementTypeMismatchException : QuantityMeasurementException
{
    public string FirstType  { get; }
    public string SecondType { get; }

    public MeasurementTypeMismatchException(string firstType, string secondType)
        : base($"Measurement type mismatch: '{firstType}' vs '{secondType}'", "TYPE_MISMATCH")
    {
        FirstType  = firstType;
        SecondType = secondType;
    }
}
