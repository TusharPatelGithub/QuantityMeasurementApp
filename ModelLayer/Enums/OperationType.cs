namespace ModelLayer.Enums;

/// <summary>
/// Represents the type of operation performed in a quantity measurement.
/// Added in UC17 alongside the existing ArithmeticOperation enum.
/// </summary>
public enum OperationType
{
    COMPARE,
    CONVERT,
    ADD,
    SUBTRACT,
    DIVIDE
}
