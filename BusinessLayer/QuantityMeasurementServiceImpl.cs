using ModelLayer.Models;
using ModelLayer.Enums;
using ModelLayer.DTOs;
using ModelLayer.Interfaces;
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

    // ─── Unit Parsers ─────────────────────────────────────────────────────────

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

    // ─── Core Operations ──────────────────────────────────────────────────────

    public QuantityMeasurementDTO Compare(QuantityDTO first, QuantityDTO second)
    {
        _logger.LogInformation("Compare: {V1} {U1} vs {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        try
        {
            if (first.MeasurementType != second.MeasurementType)
                throw new MeasurementTypeMismatchException(first.MeasurementType, second.MeasurementType);

            bool isEqual = first.MeasurementType switch
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

            var dto = new QuantityMeasurementDTO(
                first.MeasurementType, OperationType.COMPARE.ToString(),
                first.Value, second.Value, isEqual ? 1 : 0, first.Unit);

            _repository.SaveMeasurement(dto.ToEntity());
            _logger.LogInformation("Compare result: {Result}", isEqual);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Compare operation.");
            var errEntity = new QuantityMeasurementEntity(
                first.MeasurementType, OperationType.COMPARE.ToString(),
                first.Value, second.Value, ex.Message);
            _repository.SaveMeasurement(errEntity);
            throw;
        }
    }

    public QuantityMeasurementDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        _logger.LogInformation("Convert: {V} {U} -> {Target} [{Type}]",
            quantity.Value, quantity.Unit, targetUnit, quantity.MeasurementType);

        try
        {
            double resultValue = quantity.MeasurementType switch
            {
                "Length"      => Quantity<LengthUnit>.Convert(quantity.Value, ParseLengthUnit(quantity.Unit), ParseLengthUnit(targetUnit)),
                "Weight"      => Quantity<WeightUnit>.Convert(quantity.Value, ParseWeightUnit(quantity.Unit), ParseWeightUnit(targetUnit)),
                "Volume"      => Quantity<VolumeUnit>.Convert(quantity.Value, ParseVolumeUnit(quantity.Unit), ParseVolumeUnit(targetUnit)),
                "Temperature" => Quantity<TemperatureUnit>.Convert(quantity.Value, ParseTemperatureUnit(quantity.Unit), ParseTemperatureUnit(targetUnit)),
                _ => throw new InvalidMeasurementTypeException(quantity.MeasurementType)
            };

            var dto = new QuantityMeasurementDTO(
                quantity.MeasurementType, OperationType.CONVERT.ToString(),
                quantity.Value, 0, resultValue, targetUnit);

            _repository.SaveMeasurement(dto.ToEntity());
            _logger.LogInformation("Convert result: {Result} {Target}", resultValue, targetUnit);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Convert operation.");
            var errEntity = new QuantityMeasurementEntity(
                quantity.MeasurementType, OperationType.CONVERT.ToString(),
                quantity.Value, 0, ex.Message);
            _repository.SaveMeasurement(errEntity);
            throw;
        }
    }

    public QuantityMeasurementDTO Add(QuantityDTO first, QuantityDTO second)
    {
        _logger.LogInformation("Add: {V1} {U1} + {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        try
        {
            ValidateSameType(first, second);

            QuantityDTO result = first.MeasurementType switch
            {
                "Length"  => ToDTO(Quantity<LengthUnit>.Add(
                                 new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit)),
                                 new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))), "Length"),
                "Weight"  => ToDTO(Quantity<WeightUnit>.Add(
                                 new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit)),
                                 new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))), "Weight"),
                "Volume"  => ToDTO(Quantity<VolumeUnit>.Add(
                                 new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit)),
                                 new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))), "Volume"),
                "Temperature" => throw new UnsupportedOperationException("Addition", "Temperature"),
                _ => throw new InvalidMeasurementTypeException(first.MeasurementType)
            };

            var dto = new QuantityMeasurementDTO(
                first.MeasurementType, OperationType.ADD.ToString(),
                first.Value, second.Value, result.Value, result.Unit);

            _repository.SaveMeasurement(dto.ToEntity());
            _logger.LogInformation("Add result: {Result} {Unit}", result.Value, result.Unit);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Add operation.");
            var errEntity = new QuantityMeasurementEntity(
                first.MeasurementType, OperationType.ADD.ToString(),
                first.Value, second.Value, ex.Message);
            _repository.SaveMeasurement(errEntity);
            throw;
        }
    }

    public QuantityMeasurementDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        _logger.LogInformation("Subtract: {V1} {U1} - {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        try
        {
            ValidateSameType(first, second);

            QuantityDTO result = first.MeasurementType switch
            {
                "Length"  => ToDTO(new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                                 .Subtract(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))), "Length"),
                "Weight"  => ToDTO(new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                                 .Subtract(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))), "Weight"),
                "Volume"  => ToDTO(new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                                 .Subtract(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))), "Volume"),
                "Temperature" => throw new UnsupportedOperationException("Subtraction", "Temperature"),
                _ => throw new InvalidMeasurementTypeException(first.MeasurementType)
            };

            var dto = new QuantityMeasurementDTO(
                first.MeasurementType, OperationType.SUBTRACT.ToString(),
                first.Value, second.Value, result.Value, result.Unit);

            _repository.SaveMeasurement(dto.ToEntity());
            _logger.LogInformation("Subtract result: {Result} {Unit}", result.Value, result.Unit);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Subtract operation.");
            var errEntity = new QuantityMeasurementEntity(
                first.MeasurementType, OperationType.SUBTRACT.ToString(),
                first.Value, second.Value, ex.Message);
            _repository.SaveMeasurement(errEntity);
            throw;
        }
    }

    public QuantityMeasurementDTO Divide(QuantityDTO first, QuantityDTO second)
    {
        _logger.LogInformation("Divide: {V1} {U1} / {V2} {U2} [{Type}]",
            first.Value, first.Unit, second.Value, second.Unit, first.MeasurementType);

        try
        {
            ValidateSameType(first, second);

            double result = first.MeasurementType switch
            {
                "Length"      => new Quantity<LengthUnit>(first.Value, ParseLengthUnit(first.Unit))
                                     .Divide(new Quantity<LengthUnit>(second.Value, ParseLengthUnit(second.Unit))),
                "Weight"      => new Quantity<WeightUnit>(first.Value, ParseWeightUnit(first.Unit))
                                     .Divide(new Quantity<WeightUnit>(second.Value, ParseWeightUnit(second.Unit))),
                "Volume"      => new Quantity<VolumeUnit>(first.Value, ParseVolumeUnit(first.Unit))
                                     .Divide(new Quantity<VolumeUnit>(second.Value, ParseVolumeUnit(second.Unit))),
                "Temperature" => throw new UnsupportedOperationException("Division", "Temperature"),
                _ => throw new InvalidMeasurementTypeException(first.MeasurementType)
            };

            var dto = new QuantityMeasurementDTO(
                first.MeasurementType, OperationType.DIVIDE.ToString(),
                first.Value, second.Value, result, first.Unit);

            _repository.SaveMeasurement(dto.ToEntity());
            _logger.LogInformation("Divide result: {Result}", result);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Divide operation.");
            var errEntity = new QuantityMeasurementEntity(
                first.MeasurementType, OperationType.DIVIDE.ToString(),
                first.Value, second.Value, ex.Message);
            _repository.SaveMeasurement(errEntity);
            throw;
        }
    }

    // ─── UC17: Data Retrieval Methods ─────────────────────────────────────────

    public List<QuantityMeasurementDTO> GetMeasurementsByType(string measurementType)
    {
        _logger.LogInformation("GetMeasurementsByType: {Type}", measurementType);
        return QuantityMeasurementDTO.FromEntityList(_repository.GetMeasurementsByType(measurementType));
    }

    public List<QuantityMeasurementDTO> GetMeasurementsByOperation(string operationType)
    {
        _logger.LogInformation("GetMeasurementsByOperation: {Op}", operationType);
        return QuantityMeasurementDTO.FromEntityList(_repository.GetMeasurementsByOperation(operationType));
    }

    public int GetOperationCount(string operationType)
    {
        _logger.LogInformation("GetOperationCount: {Op}", operationType);
        return _repository.CountByOperation(operationType);
    }

    public List<QuantityMeasurementDTO> GetErrorMeasurements()
    {
        _logger.LogInformation("GetErrorMeasurements called.");
        return QuantityMeasurementDTO.FromEntityList(_repository.GetErrorMeasurements());
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static QuantityDTO ToDTO<U>(Quantity<U> q, string type) where U : class, IMeasurable
        => new QuantityDTO(q.MeasurementValue, q.Unit.ToString()!, type);

    private static void ValidateSameType(QuantityDTO first, QuantityDTO second)
    {
        if (first.MeasurementType != second.MeasurementType)
            throw new MeasurementTypeMismatchException(first.MeasurementType, second.MeasurementType);
    }
}
