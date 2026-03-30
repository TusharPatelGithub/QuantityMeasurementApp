using ModelLayer.Interfaces;

namespace ModelLayer.Enums;

public sealed class VolumeUnit : IMeasurable
{
    public static readonly VolumeUnit LITRE      = new VolumeUnit("LITRE",      "L",   1.0);
    public static readonly VolumeUnit MILLILITRE = new VolumeUnit("MILLILITRE", "mL",  0.001);
    public static readonly VolumeUnit GALLON     = new VolumeUnit("GALLON",     "gal", 3.78541);

    private readonly string name;
    private readonly string unitLabel;
    private readonly double conversionFactor;

    private VolumeUnit(string name, string unitLabel, double conversionFactor)
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

