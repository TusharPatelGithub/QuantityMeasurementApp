using ModelLayer.Interfaces;

namespace ModelLayer.Enums;

public sealed class WeightUnit : IMeasurable
{
    public static readonly WeightUnit KILOGRAM = new WeightUnit("KILOGRAM", "kg",  1.0);
    public static readonly WeightUnit GRAM     = new WeightUnit("GRAM",     "g",   0.001);
    public static readonly WeightUnit POUND    = new WeightUnit("POUND",    "lb",  0.453592);

    private readonly string name;
    private readonly string unitLabel;
    private readonly double conversionFactor;

    private WeightUnit(string name, string unitLabel, double conversionFactor)
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

