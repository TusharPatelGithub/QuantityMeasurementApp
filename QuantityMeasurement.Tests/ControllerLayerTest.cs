using ModelLayer.DTOs;
using BusinessLayer.Services;
using ModelLayer.Models;
using QuantityMeasurementApp.Controllers;

namespace QuantityMeasurement.Tests;

// ── In-memory mock service (no business logic needed for controller tests) ───
public class MockQuantityMeasurementService : IQuantityMeasurementService
{
    // Track calls for verification
    public List<string> CallLog { get; } = new();

    public QuantityMeasurementDTO Compare(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Compare");
        bool result = false;
        if (first.MeasurementType == second.MeasurementType)
            result = first.Value == second.Value && first.Unit == second.Unit;
            
        return new QuantityMeasurementDTO(first.MeasurementType, "COMPARE", first.Value, second.Value, result ? 1 : 0, first.Unit);
    }

    public QuantityMeasurementDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        CallLog.Add("Convert");
        return new QuantityMeasurementDTO(quantity.MeasurementType, "CONVERT", quantity.Value, 0, quantity.Value * 2, targetUnit);
    }

    public QuantityMeasurementDTO Add(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Add");
        return new QuantityMeasurementDTO(first.MeasurementType, "ADD", first.Value, second.Value, first.Value + second.Value, first.Unit);
    }

    public QuantityMeasurementDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Subtract");
        return new QuantityMeasurementDTO(first.MeasurementType, "SUBTRACT", first.Value, second.Value, first.Value - second.Value, first.Unit);
    }

    public QuantityMeasurementDTO Divide(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Divide");
        if (second.Value == 0) throw new ArithmeticException("Cannot divide by zero");
        return new QuantityMeasurementDTO(first.MeasurementType, "DIVIDE", first.Value, second.Value, first.Value / second.Value, first.Unit);
    }

    public List<QuantityMeasurementDTO> GetMeasurementsByType(string measurementType) => new();
    public List<QuantityMeasurementDTO> GetMeasurementsByOperation(string operationType) => new();
    public int GetOperationCount(string operationType) => 0;
    public List<QuantityMeasurementDTO> GetErrorMeasurements() => new();
}

// ─── Controller Tests ──────────────────────────────────────────────────────────

// Minimal test structure since the API endpoints just delegate to the service
public class ControllerLayerTest
{
    private readonly QuantityMeasurementApiController _controller;
    private readonly MockQuantityMeasurementService _mockService;
    private readonly Microsoft.Extensions.Logging.Abstractions.NullLogger<QuantityMeasurementApiController> _logger;

    public ControllerLayerTest()
    {
        _mockService = new MockQuantityMeasurementService();
        _logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<QuantityMeasurementApiController>();
        _controller = new QuantityMeasurementApiController(_mockService, _logger);
    }

    [Fact]
    public void TestController_Compare_DelegatesToService()
    {
        var input = new QuantityInputDTO {
            First = new QuantityDTO(1.0, "FEET", "Length"),
            Second = new QuantityDTO(1.0, "FEET", "Length")
        };
        _controller.CompareQuantities(input);
        Assert.Contains("Compare", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Convert_DelegatesToService()
    {
        var input = new QuantityInputDTO {
            First = new QuantityDTO(1.0, "FEET", "Length"),
            TargetUnit = "INCH"
        };
        _controller.ConvertQuantity(input);
        Assert.Contains("Convert", _mockService.CallLog);
    }
}
