using ModelLayer.Models;
using BusinessLayer.Exceptions;
using ModelLayer.Entities;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Interfaces;

namespace BusinessLayer.Services;

public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
{
    private readonly IQuantityMeasurementRepository _repository;
    private readonly ILogger<QuantityMeasurementServiceImpl> _logger;

    public QuantityMeasurementServiceImpl(
        IQuantityMeasurementRepository repository,
        ILogger<QuantityMeasurementServiceImpl>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? new Microsoft.Extensions.Logging.Abstractions.NullLogger<QuantityMeasurementServiceImpl>();
        _logger.LogInformation("QuantityMeasurementServiceImpl initialized.");
    }

    private static LengthUnit ParseLengthUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "FEET"        => LengthUnit.FEET,
        "INCH"        => LengthUnit.INCH,
        "YARDS"       => LengthUnit.YARDS,
        "CENTIMETERS" => LengthUnit.CENTIMETERS,
        _ => throw new InvalidUnitException(unit, "Length")
    };

    private static WeightUnit ParseWeightUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "KILOGRAM" => WeightUnit.KILOGRAM,
        "GRAM"     => WeightUnit.GRAM,
        "POUND"    => WeightUnit.POUND,
        _ => throw new InvalidUnitException(unit, "Weight")
    };

    private static VolumeUnit ParseVolumeUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "LITRE"      => VolumeUnit.LITRE,
        "MILLILITRE" => VolumeUnit.MILLILITRE,
        "GALLON"     => VolumeUnit.GALLON,
        _ => throw new InvalidUnitException(unit, "Volume")
    };

    private static TemperatureUnit ParseTemperatureUnit(string unit) => unit.ToUpperInvariant() switch
    {
        "CELSIUS"    => TemperatureUnit.CELSIUS,
        "FAHRENHEIT" => TemperatureUnit.FAHRENHEIT,
        _ => throw new InvalidUnitException(unit, "Temperature")
    };

    public bool Compare(QuantityDTO first, QuantityDTO second)
    {
        _logger.LogInformation("Compare: {V1} {U1} vs {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        if (first.MeasurementType != second.MeasurementType)
        {
            _logger.LogWarning("Measurement type mismatch: {T1} vs {T2}", first.MeasurementType, second.MeasurementType);
            return false;
        }

        bool result = first.MeasurementType switch
        {
            "Length"      => new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                                 .Equals(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))),
            "Weight"      => new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                                 .Equals(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))),
            "Volume"      => new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                                 .Equals(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))),
            "Temperature" => new Quantity<TemperatureUnit>(first.Value, ParseTemperatureUnit(first.Unit))
                                 .Equals(new Quantity<TemperatureUnit>(second.Value, ParseTemperatureUnit(second.Unit))),
            _ => throw new InvalidMeasurementTypeException(first.MeasurementType)
        };

        _logger.LogInformation("Compare result: {Result}", result);
        _repository.SaveMeasurement(new QuantityMeasurementEntity(
            first.MeasurementType, "Compare",
            first.Value, second.Value,
            result ? 1 : 0, first.Unit));
        return result;
    }

    public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        _logger.LogInformation("Convert: {V} {U} -> {Target} [{Type}]",
            quantity.Value, quantity.Unit, targetUnit, quantity.MeasurementType);

        double resultValue = quantity.MeasurementType switch
        {
            "Length"      => Quantity<LengthUnit>.Convert(quantity.Value, ParseLengthUnit(quantity.Unit), ParseLengthUnit(targetUnit)),
            "Weight"      => Quantity<WeightUnit>.Convert(quantity.Value, ParseWeightUnit(quantity.Unit), ParseWeightUnit(targetUnit)),
            "Volume"      => Quantity<VolumeUnit>.Convert(quantity.Value, ParseVolumeUnit(quantity.Unit), ParseVolumeUnit(targetUnit)),
            "Temperature" => Quantity<TemperatureUnit>.Convert(quantity.Value, ParseTemperatureUnit(quantity.Unit), ParseTemperatureUnit(targetUnit)),
            _ => throw new NotSupportedException($"Unsupported measurement type: '{quantity.MeasurementType}'")
        };

        _logger.LogInformation("Convert result: {Result} {Target}", resultValue, targetUnit);
        _repository.SaveMeasurement(new QuantityMeasurementEntity(
            quantity.MeasurementType, "Convert",
            quantity.Value, 0, resultValue, targetUnit));
        return new QuantityDTO(resultValue, targetUnit, quantity.MeasurementType);
    }

    public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);
        _logger.LogInformation("Add: {V1} {U1} + {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        QuantityDTO dto = first.MeasurementType switch
        {
            "Length" => ToDTO(Quantity<LengthUnit>.Add(
                            new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit)),
                            new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))), "Length"),
            "Weight" => ToDTO(Quantity<WeightUnit>.Add(
                            new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit)),
                            new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))), "Weight"),
            "Volume" => ToDTO(Quantity<VolumeUnit>.Add(
                            new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit)),
                            new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))), "Volume"),
            "Temperature" => throw new UnsupportedOperationException("Addition", "Temperature"),
            _ => throw new NotSupportedException($"Addition not supported for type: '{first.MeasurementType}'")
        };

        _logger.LogInformation("Add result: {Result} {Unit}", dto.Value, dto.Unit);
        _repository.SaveMeasurement(new QuantityMeasurementEntity(
            first.MeasurementType, "Add", first.Value, second.Value, dto.Value, dto.Unit));
        return dto;
    }

    public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);
        _logger.LogInformation("Subtract: {V1} {U1} - {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        QuantityDTO dto = first.MeasurementType switch
        {
            "Length" => ToDTO(new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                            .Subtract(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))), "Length"),
            "Weight" => ToDTO(new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                            .Subtract(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))), "Weight"),
            "Volume" => ToDTO(new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                            .Subtract(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))), "Volume"),
            "Temperature" => throw new UnsupportedOperationException("Subtraction", "Temperature"),
            _ => throw new NotSupportedException($"Subtraction not supported for type: '{first.MeasurementType}'")
        };

        _logger.LogInformation("Subtract result: {Result} {Unit}", dto.Value, dto.Unit);
        _repository.SaveMeasurement(new QuantityMeasurementEntity(
            first.MeasurementType, "Subtract", first.Value, second.Value, dto.Value, dto.Unit));
        return dto;
    }

    public double Divide(QuantityDTO first, QuantityDTO second)
    {
        ValidateSameType(first, second);
        _logger.LogInformation("Divide: {V1} {U1} / {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        double result = first.MeasurementType switch
        {
            "Length"      => new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                                 .Divide(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))),
            "Weight"      => new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                                 .Divide(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))),
            "Volume"      => new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                                 .Divide(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))),
            "Temperature" => throw new UnsupportedOperationException("Division", "Temperature"),
            _ => throw new NotSupportedException($"Division not supported for type: '{first.MeasurementType}'")
        };

        _logger.LogInformation("Divide result: {Result}", result);
        _repository.SaveMeasurement(new QuantityMeasurementEntity(
            first.MeasurementType, "Divide", first.Value, second.Value, result, first.Unit));
        return result;
    }

    private static QuantityDTO ToDTO<U>(Quantity<U> q, string type) where U : class, IMeasurable
        => new QuantityDTO(q.MeasurementValue, q.Unit.ToString()!, type);

    private static void ValidateSameType(QuantityDTO first, QuantityDTO second)
    {
        if (first.MeasurementType != second.MeasurementType)
            throw new MeasurementTypeMismatchException(first.MeasurementType, second.MeasurementType);
    }
}

