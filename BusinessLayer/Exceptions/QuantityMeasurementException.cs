namespace BusinessLayer.Exceptions;

public class QuantityMeasurementException : Exception
{
    public string ErrorCode { get; }

    public QuantityMeasurementException(string message, string errorCode = "QM_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public QuantityMeasurementException(string message, Exception innerException, string errorCode = "QM_ERROR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
