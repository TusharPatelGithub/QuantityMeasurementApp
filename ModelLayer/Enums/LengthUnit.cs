using ModelLayer.Interfaces;

namespace ModelLayer.Enums;

public sealed class LengthUnit : IMeasurable
{
    public static readonly LengthUnit FEET        = new LengthUnit("FEET",        "feet", 1.0);
    public static readonly LengthUnit INCH        = new LengthUnit("INCH",        "inch", 1.0 / 12.0);
    public static readonly LengthUnit YARDS       = new LengthUnit("YARDS",       "yards", 3.0);
    public static readonly LengthUnit CENTIMETERS = new LengthUnit("CENTIMETERS", "cm", 0.393701 / 12.0);

    private readonly string name;
    private readonly string unitLabel;
    private readonly double conversionFactor;

    private LengthUnit(string name, string unitLabel, double conversionFactor)
    {
        this.name             = name;
        this.unitLabel        = unitLabel;
        this.conversionFactor = conversionFactor;
    }

    public double GetConversionFactor()            => this.conversionFactor;
    public double ConvertToBaseUnit(double value)  => value * this.conversionFactor;
    public double ConvertFromBaseUnit(double baseValue) => baseValue / this.conversionFactor;
    public string GetUnitName()                    => this.unitLabel;
    public override string ToString()              => this.name;
}

