using ModelLayer.Entities;
using RepositoryLayer.DatabaseRepository;
using RepositoryLayer.Interfaces;

namespace QuantityMeasurement.Tests;

public class RepositoryLayerTest : IDisposable
{
    private readonly IQuantityMeasurementRepository _repository;

    public RepositoryLayerTest()
    {
        _repository = new QuantityMeasurementDatabaseRepository();
        // Clean slate before each test
        _repository.DeleteAll();
    }

    public void Dispose()
    {
        // Clean up after each test
        _repository.DeleteAll();
    }

    // ==================== SaveMeasurement Tests ====================

    [Fact]
    public void TestSaveMeasurement_ValidEntity_SavesSuccessfully()
    {
        var entity = new QuantityMeasurementEntity("Length", "Compare", 1.0, 12.0, 1.0, "FEET");
        _repository.SaveMeasurement(entity);
        Assert.Equal(1, _repository.GetTotalCount());
    }

    [Fact]
    public void TestSaveMeasurement_MultipleEntities_AllSaved()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Compare", 1.0, 12.0, 1.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Volume", "Convert", 1.0, 0.0, 1000.0, "MILLILITRE"));
        Assert.Equal(3, _repository.GetTotalCount());
    }

    [Fact]
    public void TestSaveMeasurement_PreservesOperationType()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        var results = _repository.GetMeasurementsByOperation("Add");
        Assert.Single(results);
        Assert.Equal("Add", results[0].OperationType);
    }

    [Fact]
    public void TestSaveMeasurement_PreservesMeasurementType()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Subtract", 10.0, 5.0, 5.0, "KILOGRAM"));
        var results = _repository.GetMeasurementsByType("Weight");
        Assert.Single(results);
        Assert.Equal("Weight", results[0].MeasurementType);
    }

    [Fact]
    public void TestSaveMeasurement_PreservesValues()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Divide", 10.0, 2.0, 5.0, "FEET"));
        var results = _repository.GetAllMeasurements();
        Assert.Single(results);
        Assert.Equal(10.0, results[0].Value1, 6);
        Assert.Equal(2.0, results[0].Value2, 6);
        Assert.Equal(5.0, results[0].Result, 6);
    }

    [Fact]
    public void TestSaveMeasurement_PreservesUnit()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Volume", "Add", 1.0, 1.0, 2.0, "LITRE"));
        var results = _repository.GetAllMeasurements();
        Assert.Equal("LITRE", results[0].Unit);
    }

    [Fact]
    public void TestSaveMeasurement_CreatedAtIsSet()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Compare", 1.0, 12.0, 1.0, "FEET"));
        var results = _repository.GetAllMeasurements();
        Assert.True(results[0].CreatedAt > DateTime.MinValue);
    }

    // ==================== GetAllMeasurements Tests ====================

    [Fact]
    public void TestGetAllMeasurements_EmptyRepository_ReturnsEmptyList()
    {
        var results = _repository.GetAllMeasurements();
        Assert.Empty(results);
    }

    [Fact]
    public void TestGetAllMeasurements_ReturnsAllSavedEntities()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Compare", 1.0, 1000.0, 1.0, "KILOGRAM"));
        var results = _repository.GetAllMeasurements();
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void TestGetAllMeasurements_ReturnsList_NotNull()
    {
        var results = _repository.GetAllMeasurements();
        Assert.NotNull(results);
    }

    // ==================== GetMeasurementsByOperation Tests ====================

    [Fact]
    public void TestGetMeasurementsByOperation_FiltersByOperationType()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Compare", 1.0, 1000.0, 1.0, "KILOGRAM"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Volume", "Add", 1.0, 1.0, 2.0, "LITRE"));
        var results = _repository.GetMeasurementsByOperation("Add");
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal("Add", r.OperationType));
    }

    [Fact]
    public void TestGetMeasurementsByOperation_NonExistentType_ReturnsEmpty()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        var results = _repository.GetMeasurementsByOperation("Multiply");
        Assert.Empty(results);
    }

    [Fact]
    public void TestGetMeasurementsByOperation_AllOperationTypes()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Subtract", 5.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Divide", 10.0, 2.0, 5.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Compare", 1.0, 12.0, 1.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Convert", 1.0, 0.0, 12.0, "INCH"));

        Assert.Single(_repository.GetMeasurementsByOperation("Add"));
        Assert.Single(_repository.GetMeasurementsByOperation("Subtract"));
        Assert.Single(_repository.GetMeasurementsByOperation("Divide"));
        Assert.Single(_repository.GetMeasurementsByOperation("Compare"));
        Assert.Single(_repository.GetMeasurementsByOperation("Convert"));
    }

    // ==================== GetMeasurementsByType Tests ====================

    [Fact]
    public void TestGetMeasurementsByType_FiltersByMeasurementType()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Compare", 1.0, 12.0, 1.0, "FEET"));

        var results = _repository.GetMeasurementsByType("Length");
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal("Length", r.MeasurementType));
    }

    [Fact]
    public void TestGetMeasurementsByType_NonExistentType_ReturnsEmpty()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        var results = _repository.GetMeasurementsByType("Temperature");
        Assert.Empty(results);
    }

    [Fact]
    public void TestGetMeasurementsByType_AllMeasurementTypes()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Volume", "Add", 1.0, 1.0, 2.0, "LITRE"));

        Assert.Single(_repository.GetMeasurementsByType("Length"));
        Assert.Single(_repository.GetMeasurementsByType("Weight"));
        Assert.Single(_repository.GetMeasurementsByType("Volume"));
    }

    // ==================== GetTotalCount Tests ====================

    [Fact]
    public void TestGetTotalCount_EmptyRepository_ReturnsZero()
    {
        Assert.Equal(0, _repository.GetTotalCount());
    }

    [Fact]
    public void TestGetTotalCount_AfterSaving_ReturnsCorrectCount()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        Assert.Equal(2, _repository.GetTotalCount());
    }

    [Fact]
    public void TestGetTotalCount_AfterDeleteAll_ReturnsZero()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.DeleteAll();
        Assert.Equal(0, _repository.GetTotalCount());
    }

    // ==================== DeleteAll Tests ====================

    [Fact]
    public void TestDeleteAll_ClearsAllRecords()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        _repository.DeleteAll();
        Assert.Empty(_repository.GetAllMeasurements());
    }

    [Fact]
    public void TestDeleteAll_OnEmptyRepository_DoesNotThrow()
    {
        var exception = Record.Exception(() => _repository.DeleteAll());
        Assert.Null(exception);
    }

    [Fact]
    public void TestDeleteAll_CanSaveAfterDelete()
    {
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Length", "Add", 1.0, 2.0, 3.0, "FEET"));
        _repository.DeleteAll();
        _repository.SaveMeasurement(new QuantityMeasurementEntity("Weight", "Add", 1.0, 1000.0, 2.0, "KILOGRAM"));
        Assert.Equal(1, _repository.GetTotalCount());
    }
}
