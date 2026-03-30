using ModelLayer.Interfaces;

namespace ModelLayer.Enums;

public sealed class TemperatureUnit : IMeasurable
{
    public static readonly TemperatureUnit CELSIUS = new TemperatureUnit(
        "CELSIUS", "°C",
        (celsius)    => celsius,
        (celsius)    => celsius);

    public static readonly TemperatureUnit FAHRENHEIT = new TemperatureUnit(
        "FAHRENHEIT", "°F",
        (fahrenheit) => (fahrenheit - 32.0) * 5.0 / 9.0,
        (celsius)    => celsius * 9.0 / 5.0 + 32.0);

    private readonly string name;
    private readonly string unitLabel;
    private readonly Func<double, double> toBaseUnit;
    private readonly Func<double, double> fromBaseUnit;
    private readonly Func<bool> supportsArithmetic = () => false;

    private TemperatureUnit(string name, string unitLabel,
        Func<double, double> toBaseUnit, Func<double, double> fromBaseUnit)
    {
        this.name          = name;
        this.unitLabel     = unitLabel;
        this.toBaseUnit    = toBaseUnit;
        this.fromBaseUnit  = fromBaseUnit;
    }

    public double GetConversionFactor()            => 1.0;
    public double ConvertToBaseUnit(double value)  => this.toBaseUnit(value);
    public double ConvertFromBaseUnit(double baseValue) => this.fromBaseUnit(baseValue);
    public string GetUnitName()                    => this.unitLabel;
    public bool SupportsArithmetic()               => this.supportsArithmetic();
    public override string ToString()              => this.name;

    public void ValidateOperationSupport(string operation)
    {
        throw new InvalidOperationException(
            $"Temperature does not support {operation} operation. " +
            "Temperature measurements use non-linear conversions and arithmetic operations are not meaningful.");
    }
}

