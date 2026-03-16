using ModelLayer.Entities;
using ModelLayer.Models;
using BusinessLayer.Services;
using RepositoryLayer.Interfaces;

namespace QuantityMeasurement.Tests;

// ── In-memory mock repository (no DB needed for service tests) ──────────────
public class MockQuantityMeasurementRepository : IQuantityMeasurementRepository
{
    private readonly List<QuantityMeasurementEntity> _store = new();

    public void SaveMeasurement(QuantityMeasurementEntity entity) => _store.Add(entity);

    public List<QuantityMeasurementEntity> GetAllMeasurements() => new(_store);

    public List<QuantityMeasurementEntity> GetMeasurementsByOperation(string op) =>
        _store.Where(e => e.OperationType == op).ToList();

    public List<QuantityMeasurementEntity> GetMeasurementsByType(string type) =>
        _store.Where(e => e.MeasurementType == type).ToList();

    public int GetTotalCount() => _store.Count;

    public void DeleteAll() => _store.Clear();
}

public class ServiceLayerTest
{
    private readonly IQuantityMeasurementService _service;
    private readonly MockQuantityMeasurementRepository _mockRepo;

    public ServiceLayerTest()
    {
        _mockRepo = new MockQuantityMeasurementRepository();
        _service = new QuantityMeasurementServiceImpl(_mockRepo);
    }

    // ==================== Compare Tests ====================

    [Fact]
    public void TestCompare_Length_1Feet_12Inches_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        Assert.True(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_Length_DifferentValues_ReturnsFalse()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "INCH", "Length");
        Assert.False(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_Weight_1Kg_1000Grams_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(1000.0, "GRAM", "Weight");
        Assert.True(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_Volume_1Litre_1000Ml_ReturnsTrue()
    {
        var first = new QuantityDTO(1.0, "LITRE", "Volume");
        var second = new QuantityDTO(1000.0, "MILLILITRE", "Volume");
        Assert.True(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_Temperature_100C_212F_ReturnsTrue()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(212.0, "FAHRENHEIT", "Temperature");
        Assert.True(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_DifferentMeasurementTypes_ReturnsFalse()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        Assert.False(_service.Compare(first, second));
    }

    [Fact]
    public void TestCompare_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        _service.Compare(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("Compare", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    [Fact]
    public void TestCompare_UnsupportedMeasurementType_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(1.0, "FEET", "Speed");
        var second = new QuantityDTO(1.0, "FEET", "Speed");
        Assert.Throws<NotSupportedException>(() => _service.Compare(first, second));
    }

    // ==================== Convert Tests ====================

    [Fact]
    public void TestConvert_Length_1Feet_To_Inch_Returns12()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        var result = _service.Convert(quantity, "INCH");
        Assert.Equal(12.0, result.Value, 6);
        Assert.Equal("INCH", result.Unit);
        Assert.Equal("Length", result.MeasurementType);
    }

    [Fact]
    public void TestConvert_Weight_1Kg_To_Gram_Returns1000()
    {
        var quantity = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var result = _service.Convert(quantity, "GRAM");
        Assert.Equal(1000.0, result.Value, 6);
        Assert.Equal("GRAM", result.Unit);
    }

    [Fact]
    public void TestConvert_Volume_1Litre_To_Millilitre_Returns1000()
    {
        var quantity = new QuantityDTO(1.0, "LITRE", "Volume");
        var result = _service.Convert(quantity, "MILLILITRE");
        Assert.Equal(1000.0, result.Value, 6);
        Assert.Equal("MILLILITRE", result.Unit);
    }

    [Fact]
    public void TestConvert_Temperature_100C_To_Fahrenheit_Returns212()
    {
        var quantity = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var result = _service.Convert(quantity, "FAHRENHEIT");
        Assert.Equal(212.0, result.Value, 4);
        Assert.Equal("FAHRENHEIT", result.Unit);
    }

    [Fact]
    public void TestConvert_SavesToDatabaseRepository()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        _service.Convert(quantity, "INCH");
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("Convert", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    [Fact]
    public void TestConvert_UnknownUnit_ThrowsNotSupportedException()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        Assert.Throws<NotSupportedException>(() => _service.Convert(quantity, "MILES"));
    }

    // ==================== Add Tests ====================

    [Fact]
    public void TestAdd_Length_1Feet_12Inches_Returns2Feet()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        var result = _service.Add(first, second);
        Assert.Equal(2.0, result.Value, 6);
        Assert.Equal("Length", result.MeasurementType);
    }

    [Fact]
    public void TestAdd_Weight_1Kg_1000Grams_Returns2Kg()
    {
        var first = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(1000.0, "GRAM", "Weight");
        var result = _service.Add(first, second);
        Assert.Equal(2.0, result.Value, 6);
    }

    [Fact]
    public void TestAdd_Volume_1Litre_1000Ml_Returns2Litres()
    {
        var first = new QuantityDTO(1.0, "LITRE", "Volume");
        var second = new QuantityDTO(1000.0, "MILLILITRE", "Volume");
        var result = _service.Add(first, second);
        Assert.Equal(2.0, result.Value, 6);
    }

    [Fact]
    public void TestAdd_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<NotSupportedException>(() => _service.Add(first, second));
    }

    [Fact]
    public void TestAdd_MismatchedTypes_ThrowsArgumentException()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        Assert.Throws<ArgumentException>(() => _service.Add(first, second));
    }

    [Fact]
    public void TestAdd_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(12.0, "INCH", "Length");
        _service.Add(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("Add", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    // ==================== Subtract Tests ====================

    [Fact]
    public void TestSubtract_Length_10Feet_6Inches_Returns9Point5Feet()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(6.0, "INCH", "Length");
        var result = _service.Subtract(first, second);
        Assert.Equal(9.5, result.Value, 6);
    }

    [Fact]
    public void TestSubtract_Weight_10Kg_5000Grams_Returns5Kg()
    {
        var first = new QuantityDTO(10.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(5000.0, "GRAM", "Weight");
        var result = _service.Subtract(first, second);
        Assert.Equal(5.0, result.Value, 6);
    }

    [Fact]
    public void TestSubtract_Volume_5Litres_2Litres_Returns3Litres()
    {
        var first = new QuantityDTO(5.0, "LITRE", "Volume");
        var second = new QuantityDTO(2.0, "LITRE", "Volume");
        var result = _service.Subtract(first, second);
        Assert.Equal(3.0, result.Value, 6);
    }

    [Fact]
    public void TestSubtract_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<NotSupportedException>(() => _service.Subtract(first, second));
    }

    [Fact]
    public void TestSubtract_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(6.0, "INCH", "Length");
        _service.Subtract(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("Subtract", _mockRepo.GetAllMeasurements()[0].OperationType);
    }

    // ==================== Divide Tests ====================

    [Fact]
    public void TestDivide_Length_10Feet_5Feet_Returns2()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        Assert.Equal(2.0, _service.Divide(first, second), 6);
    }

    [Fact]
    public void TestDivide_Weight_10Kg_5Kg_Returns2()
    {
        var first = new QuantityDTO(10.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(5.0, "KILOGRAM", "Weight");
        Assert.Equal(2.0, _service.Divide(first, second), 6);
    }

    [Fact]
    public void TestDivide_Volume_10Litres_5Litres_Returns2()
    {
        var first = new QuantityDTO(10.0, "LITRE", "Volume");
        var second = new QuantityDTO(5.0, "LITRE", "Volume");
        Assert.Equal(2.0, _service.Divide(first, second), 6);
    }

    [Fact]
    public void TestDivide_Temperature_ThrowsNotSupportedException()
    {
        var first = new QuantityDTO(100.0, "CELSIUS", "Temperature");
        var second = new QuantityDTO(50.0, "CELSIUS", "Temperature");
        Assert.Throws<NotSupportedException>(() => _service.Divide(first, second));
    }

    [Fact]
    public void TestDivide_MismatchedTypes_ThrowsArgumentException()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "KILOGRAM", "Weight");
        Assert.Throws<ArgumentException>(() => _service.Divide(first, second));
    }

    [Fact]
    public void TestDivide_SavesToDatabaseRepository()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        _service.Divide(first, second);
        Assert.Equal(1, _mockRepo.GetTotalCount());
        Assert.Equal("Divide", _mockRepo.GetAllMeasurements()[0].OperationType);
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

        Assert.Single(_mockRepo.GetMeasurementsByOperation("Compare"));
        Assert.Single(_mockRepo.GetMeasurementsByOperation("Convert"));
        Assert.Single(_mockRepo.GetMeasurementsByOperation("Add"));
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
