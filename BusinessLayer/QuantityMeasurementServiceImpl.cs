using ModelLayer.Models;

namespace BusinessLayer.Services;

public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
{
    // ── Unit string → instance resolvers ──────────────────────────────────────

    private static LengthUnit ParseLengthUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "FEET"        => LengthUnit.FEET,
        "INCH"        => LengthUnit.INCH,
        "YARDS"       => LengthUnit.YARDS,
        "CENTIMETERS" => LengthUnit.CENTIMETERS,
        _ => throw new NotSupportedException($"Unknown length unit: '{unit}'")
    };

    private static WeightUnit ParseWeightUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "KILOGRAM" => WeightUnit.KILOGRAM,
        "GRAM"     => WeightUnit.GRAM,
        "POUND"    => WeightUnit.POUND,
        _ => throw new NotSupportedException($"Unknown weight unit: '{unit}'")
    };

    private static VolumeUnit ParseVolumeUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "LITRE"      => VolumeUnit.LITRE,
        "MILLILITRE" => VolumeUnit.MILLILITRE,
        "GALLON"     => VolumeUnit.GALLON,
        _ => throw new NotSupportedException($"Unknown volume unit: '{unit}'")
    };

    private static TemperatureUnit ParseTemperatureUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "CELSIUS"    => TemperatureUnit.CELSIUS,
        "FAHRENHEIT" => TemperatureUnit.FAHRENHEIT,
        _ => throw new NotSupportedException($"Unknown temperature unit: '{unit}'")
    };

    // ── Compare ───────────────────────────────────────────────────────────────

    public bool Compare(QuantityDTO first, QuantityDTO second)
    {
        if (first.MeasurementType != second.MeasurementType)
            return false;

        return first.MeasurementType switch
        {
            "Length" => new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                            .Equals(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))),

            "Weight" => new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                            .Equals(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))),

            "Volume" => new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                            .Equals(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))),

            "Temperature" => new Quantity<TemperatureUnit>(first.Value, ParseTemperatureUnit(first.Unit))
                                 .Equals(new Quantity<TemperatureUnit>(second.Value, ParseTemperatureUnit(second.Unit))),

            _ => throw new NotSupportedException($"Unsupported measurement type: '{first.MeasurementType}'")
        };
    }

    // ── Convert ───────────────────────────────────────────────────────────────

    public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        double result = quantity.MeasurementType switch
        {
            "Length" => Quantity<LengthUnit>.Convert(
                            quantity.Value,
                            ParseLengthUnit(quantity.Unit),
                            ParseLengthUnit(targetUnit)),

            "Weight" => Quantity<WeightUnit>.Convert(
                            quantity.Value,
                            ParseWeightUnit(quantity.Unit),
                            ParseWeightUnit(targetUnit)),

            "Volume" => Quantity<VolumeUnit>.Convert(
                            quantity.Value,
                            ParseVolumeUnit(quantity.Unit),
                            ParseVolumeUnit(targetUnit)),

            "Temperature" => Quantity<TemperatureUnit>.Convert(
                                 quantity.Value,
                                 ParseTemperatureUnit(quantity.Unit),
                                 ParseTemperatureUnit(targetUnit)),

            _ => throw new NotSupportedException($"Unsupported measurement type: '{quantity.MeasurementType}'")
        };

        return new QuantityDTO(result, targetUnit, quantity.MeasurementType);
    }

    // ── Add ───────────────────────────────────────────────────────────────────

    public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);

        switch (first.MeasurementType)
        {
            case "Length":
            {
                var q1     = new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit));
                var q2     = new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit));
                var result = Quantity<LengthUnit>.Add(q1, q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Length");
            }
            case "Weight":
            {
                var q1     = new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit));
                var q2     = new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit));
                var result = Quantity<WeightUnit>.Add(q1, q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Weight");
            }
            case "Volume":
            {
                var q1     = new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit));
                var q2     = new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit));
                var result = Quantity<VolumeUnit>.Add(q1, q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Volume");
            }
            case "Temperature":
                throw new NotSupportedException("Addition is not supported for Temperature measurements.");

            default:
                throw new NotSupportedException($"Addition not supported for type: '{first.MeasurementType}'");
        }
    }

    // ── Subtract ──────────────────────────────────────────────────────────────

    public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);

        switch (first.MeasurementType)
        {
            case "Length":
            {
                var q1     = new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit));
                var q2     = new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit));
                var result = q1.Subtract(q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Length");
            }
            case "Weight":
            {
                var q1     = new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit));
                var q2     = new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit));
                var result = q1.Subtract(q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Weight");
            }
            case "Volume":
            {
                var q1     = new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit));
                var q2     = new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit));
                var result = q1.Subtract(q2);
                return new QuantityDTO(result.MeasurementValue, result.Unit.ToString(), "Volume");
            }
            case "Temperature":
                throw new NotSupportedException("Subtraction is not supported for Temperature measurements.");

            default:
                throw new NotSupportedException($"Subtraction not supported for type: '{first.MeasurementType}'");
        }
    }

    // ── Divide ────────────────────────────────────────────────────────────────

    public double Divide(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);

        return first.MeasurementType switch
        {
            "Length" => new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                            .Divide(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))),

            "Weight" => new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                            .Divide(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))),

            "Volume" => new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                            .Divide(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))),

            "Temperature" => throw new NotSupportedException("Division is not supported for Temperature measurements."),

            _ => throw new NotSupportedException($"Division not supported for type: '{first.MeasurementType}'")
        };
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static void ValidateSameType(QuantityDTO first, QuantityDTO second)
    {
        if (first.MeasurementType != second.MeasurementType)
            throw new ArgumentException(
                $"Measurement type mismatch: '{first.MeasurementType}' vs '{second.MeasurementType}'");
    }
}