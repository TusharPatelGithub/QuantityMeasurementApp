namespace ModelLayer.Interfaces;

public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();
    bool SupportsArithmetic() => true;
    void ValidateOperationSupport(string operation) { }
}
