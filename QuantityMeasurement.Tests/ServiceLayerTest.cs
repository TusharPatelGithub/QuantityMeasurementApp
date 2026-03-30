using BusinessLayer.Exceptions;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using ModelLayer.Models;
using BusinessLayer.Services;
using RepositoryLayer.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace QuantityMeasurement.Tests;

// ── In-memory mock repository (no DB needed for service tests) ──────────────
public class MockQuantityMeasurementRepository : IQuantityMeasurementRepository
{
    private readonly List<QuantityMeasurementEntity> _store = new();

    public void SaveMeasurement(QuantityMeasurementEntity entity) => _store.Add(entity);

    public List<QuantityMeasurementEntity> GetAllMeasurements() => new(_store);

    public List<QuantityMeasurementEntity> GetMeasurementsByOperation(string operationType) =>
        _store.Where(e => e.OperationType == operationType).ToList();

    public List<QuantityMeasurementEntity> GetMeasurementsByType(string measurementType) =>
        _store.Where(e => e.MeasurementType == measurementType).ToList();

    public int GetTotalCount() => _store.Count;

    public int CountByOperation(string operationType) =>
        _store.Count(e => e.OperationType == operationType);

    public List<QuantityMeasurementEntity> GetErrorMeasurements() =>
        _store.Where(e => e.HasError).ToList();

    public void DeleteAll() => _store.Clear();
}

public class ServiceLayerTest
{
    private readonly IQuantityMeasurementService _service;
    private readonly MockQuantityMeasurementRepository _mockRepo;

    public ServiceLayerTest()
    {
        _mockRepo = new MockQuantityMeasurementRepository();
        _service = new QuantityMeasurementServiceImpl(_mockRepo, new NullLogger<QuantityMeasurementServiceImpl>());
    }

    // ==================== Compare Tests ====================

    [Fact]
    public void TestCompare_Length_1Feet_12Inches_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        var result = _service.Compare(first, second);
        Assert.NotNull(result);
        Assert.Equal(1, result.Result); // Result is 1 for equal
    }

    [Fact]
    public void TestCompare_Length_DifferentValues_ReturnsFalse()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "INCH", "Length");
        var result = _service.Compare(first, second);
        Assert.NotNull(result);
        Assert.Equal(0, result.Result); // Result is 0 for not equal
    }

    [Fact]
    public void TestCompare_Weight_1Kg_1000Grams_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(1000.0, "GRAM", "Weight");
        var result = _service.Compare(first, second);
        Assert.NotNull(result);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public void TestCompare_Volume_1Litre_1000Ml_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "LITRE", "Volume");
        var second = new QuantityDTO(1000.0, "MILLILITRE", "Volume");
        var result = _service.Compare(first, second);
        Assert.NotNull(result);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public void TestCompare_Temperature_100C_212F_ReturnsTrue()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(212.0, "FAHRENHEIT", "Temperature");
        var result = _service.Compare(first, second);
        Assert.NotNull(result);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public void TestCompare_DifferentMeasurementTypes_ReturnsFalse()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        Assert.Throws<MeasurementTypeMismatchException>(() => _service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        _service.Compare(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("COMPARE", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    [Fact]
    public void TestCompare_UnsupportedMeasurementType_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(1.0, "FEET", "Speed");
        var second = new QuantityDTO(1.0, "FEET", "Speed");
        Assert.Throws<InvalidMeasurementTypeException>(() => _service.Compare(first, second));
    }

    // ==================== Convert Tests ====================

    [Fact]
    public void TestConvert_Length_1Feet_To_Inch_Returns12()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        var result = _service.Convert(quantity, "INCH");
        Assert.NotNull(result);
        Assert.Equal(12.0, result.Result, 6);
        Assert.Equal("INCH", result.Unit);
        Assert.Equal("Length", result.MeasurementType);
    }

    [Fact]
    public void TestConvert_Weight_1Kg_To_Gram_Returns1000()
    {
        var quantity = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var result = _service.Convert(quantity, "GRAM");
        Assert.NotNull(result);
        Assert.Equal(1000.0, result.Result, 6);
        Assert.Equal("GRAM", result.Unit);
    }

    [Fact]
    public void TestConvert_Volume_1Litre_To_Millilitre_Returns1000()
    {
        var quantity = new QuantityDTO(1.0, "LITRE", "Volume");
        var result = _service.Convert(quantity, "MILLILITRE");
        Assert.NotNull(result);
        Assert.Equal(1000.0, result.Result, 6);
        Assert.Equal("MILLILITRE", result.Unit);
    }

    [Fact]
    public void TestConvert_Temperature_100C_To_Fahrenheit_Returns212()
    {
        var quantity = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var result = _service.Convert(quantity, "FAHRENHEIT");
        Assert.NotNull(result);
        Assert.Equal(212.0, result.Result, 4);
        Assert.Equal("FAHRENHEIT", result.Unit);
    }

    [Fact]
    public void TestConvert_SavesToDatabaseRepository()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        _service.Convert(quantity, "INCH");
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("CONVERT", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    [Fact]
    public void TestConvert_UnknownUnit_ThrowsNotSupportedException()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        Assert.Throws<InvalidUnitException>(() => _service.Convert(quantity, "MILES"));
    }

    // ==================== Add Tests ====================

    [Fact]
    public void TestAdd_Length_1Feet_12Inches_Returns2Feet()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        var result = _service.Add(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
        Assert.Equal("Length", result.MeasurementType);
    }

    [Fact]
    public void TestAdd_Weight_1Kg_1000Grams_Returns2Kg()
    {
        var first = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(1000.0, "GRAM", "Weight");
        var result = _service.Add(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
    }

    [Fact]
    public void TestAdd_Volume_1Litre_1000Ml_Returns2Litres()
    {
        var first = new QuantityDTO(1.0, "LITRE", "Volume");
        var second = new QuantityDTO(1000.0, "MILLILITRE", "Volume");
        var result = _service.Add(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
    }

    [Fact]
    public void TestAdd_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<UnsupportedOperationException>(() => _service.Add(first, second));
    }

    [Fact]
    public void TestAdd_MismatchedTypes_ThrowsArgumentException()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        Assert.Throws<MeasurementTypeMismatchException>(() => _service.Add(first, second));
    }

    [Fact]
    public void TestAdd_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        _service.Add(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("ADD", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    // ==================== Subtract Tests ====================

    [Fact]
    public void TestSubtract_Length_10Feet_6Inches_Returns9Point5Feet()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(6.0, "INCH", "Length");
        var result = _service.Subtract(first, second);
        Assert.NotNull(result);
        Assert.Equal(9.5, result.Result, 6);
    }

    [Fact]
    public void TestSubtract_Weight_10Kg_5000Grams_Returns5Kg()
    {
        var first = new QuantityDTO(10.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(5000.0, "GRAM", "Weight");
        var result = _service.Subtract(first, second);
        Assert.NotNull(result);
        Assert.Equal(5.0, result.Result, 6);
    }

    [Fact]
    public void TestSubtract_Volume_5Litres_2Litres_Returns3Litres()
    {
        var first = new QuantityDTO(5.0, "LITRE", "Volume");
        var second = new QuantityDTO(2.0, "LITRE", "Volume");
        var result = _service.Subtract(first, second);
        Assert.NotNull(result);
        Assert.Equal(3.0, result.Result, 6);
    }

    [Fact]
    public void TestSubtract_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<UnsupportedOperationException>(() => _service.Subtract(first, second));
    }

    [Fact]
    public void TestSubtract_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(6.0, "INCH", "Length");
        _service.Subtract(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("SUBTRACT", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    // ==================== Divide Tests ====================

    [Fact]
    public void TestDivide_Length_10Feet_5Feet_Returns2()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        var result = _service.Divide(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
    }

    [Fact]
    public void TestDivide_Weight_10Kg_5Kg_Returns2()
    {
        var first = new QuantityDTO(10.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(5.0, "KILOGRAM", "Weight");
        var result = _service.Divide(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
    }

    [Fact]
    public void TestDivide_Volume_10Litres_5Litres_Returns2()
    {
        var first = new QuantityDTO(10.0, "LITRE", "Volume");
        var second = new QuantityDTO(5.0, "LITRE", "Volume");
        var result = _service.Divide(first, second);
        Assert.NotNull(result);
        Assert.Equal(2.0, result.Result, 6);
    }

    [Fact]
    public void TestDivide_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<UnsupportedOperationException>(() => _service.Divide(first, second));
    }

    [Fact]
    public void TestDivide_MismatchedTypes_ThrowsArgumentException()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "KILOGRAM", "Weight");
        Assert.Throws<MeasurementTypeMismatchException>(() => _service.Divide(first, second));
    }

    [Fact]
    public void TestDivide_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        _service.Divide(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("DIVIDE", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    // ==================== Repository Persistence Tests ====================

    [Fact]
    public void TestService_AllOperations_SaveToRepository()
    {
        var length1 = new QuantityDTO(1.0, "FEET", "Length");
        var length2 = new QuantityDTO(12.0, "INCH", "Length");

        _service.Compare(length1, length2);
        _service.Convert(length1, "INCH");
        _service.Add(length1, length2);
        _service.Subtract(new QuantityDTO(10.0, "FEET", "Length"), new QuantityDTO(6.0, "INCH", "Length"));
        _service.Divide(new QuantityDTO(10.0, "FEET", "Length"), new QuantityDTO(5.0, "FEET", "Length"));

        Assert.Equal(5, _mockRepo.GetTotalCount());
    }

    [Fact]
    public void TestService_OperationTypes_SavedCorrectly()
    {
        var length1 = new QuantityDTO(1.0, "FEET", "Length");
        var length2 = new QuantityDTO(12.0, "INCH", "Length");

        _service.Compare(length1, length2);
        _service.Convert(length1, "INCH");
        _service.Add(length1, length2);

        Assert.Single(_mockRepo.GetMeasurementsByOperation("COMPARE"));
        Assert.Single(_mockRepo.GetMeasurementsByOperation("CONVERT"));
        Assert.Single(_mockRepo.GetMeasurementsByOperation("ADD"));
    }

    [Fact]
    public void TestService_MeasurementTypes_SavedCorrectly()
    {
        _service.Add(new QuantityDTO(1.0, "FEET", "Length"), new QuantityDTO(12.0, "INCH", "Length"));
        _service.Add(new QuantityDTO(1.0, "KILOGRAM", "Weight"), new QuantityDTO(1000.0, "GRAM", "Weight"));
        _service.Add(new QuantityDTO(1.0, "LITRE", "Volume"), new QuantityDTO(1000.0, "MILLILITRE", "Volume"));

        Assert.Single(_mockRepo.GetMeasurementsByType("Length"));
        Assert.Single(_mockRepo.GetMeasurementsByType("Weight"));
        Assert.Single(_mockRepo.GetMeasurementsByType("Volume"));
    }
}