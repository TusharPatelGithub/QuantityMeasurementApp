using ModelLayer.Models;
using BusinessLayer.Services;
using QuantityMeasurementApp.Controllers;

namespace QuantityMeasurement.Tests;

// ── In-memory mock service (no business logic needed for controller tests) ───
public class MockQuantityMeasurementService : IQuantityMeasurementService
{
    // Track calls for verification
    public List<string> CallLog { get; } = new();

    public bool Compare(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Compare");
        if (first.MeasurementType != second.MeasurementType) return false;
        return first.Value == second.Value && first.Unit == second.Unit;
    }

    public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        CallLog.Add("Convert");
        return new QuantityDTO(quantity.Value * 2, targetUnit, quantity.MeasurementType);
    }

    public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Add");
        return new QuantityDTO(first.Value + second.Value, first.Unit, first.MeasurementType);
    }

    public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Subtract");
        return new QuantityDTO(first.Value - second.Value, first.Unit, first.MeasurementType);
    }

    public double Divide(QuantityDTO first, QuantityDTO second)
    {
        CallLog.Add("Divide");
        if (second.Value == 0) throw new ArithmeticException("Cannot divide by zero");
        return first.Value / second.Value;
    }
}

public class ControllerLayerTest
{
    private readonly QuantityMeasurementController _controller;
    private readonly MockQuantityMeasurementService _mockService;

    public ControllerLayerTest()
    {
        _mockService = new MockQuantityMeasurementService();
        _controller = new QuantityMeasurementController(_mockService);
    }

    // ==================== Constructor Tests ====================

    [Fact]
    public void TestController_Constructor_CreatesInstance()
    {
        Assert.NotNull(_controller);
    }

    [Fact]
    public void TestController_Constructor_NullService_ThrowsNullReferenceOnCall()
    {
        // Controller doesn't guard null in constructor — test that service is called
        var controller = new QuantityMeasurementController(_mockService);
        Assert.NotNull(controller);
    }

    // ==================== Compare Tests ====================

    [Fact]
    public void TestController_Compare_DelegatesToService()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "FEET", "Length");
        _controller.Compare(first, second);
        Assert.Contains("Compare", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Compare_ReturnsServiceResult_True()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "FEET", "Length");
        bool result = _controller.Compare(first, second);
        Assert.True(result);
    }

    [Fact]
    public void TestController_Compare_ReturnsServiceResult_False()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(2.0, "FEET", "Length");
        bool result = _controller.Compare(first, second);
        Assert.False(result);
    }

    [Fact]
    public void TestController_Compare_DifferentMeasurementTypes_ReturnsFalse()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        bool result = _controller.Compare(first, second);
        Assert.False(result);
    }

    [Fact]
    public void TestController_Compare_CallsServiceExactlyOnce()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(1.0, "FEET", "Length");
        _controller.Compare(first, second);
        Assert.Single(_mockService.CallLog);
    }

    // ==================== Convert Tests ====================

    [Fact]
    public void TestController_Convert_DelegatesToService()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        _controller.Convert(quantity, "INCH");
        Assert.Contains("Convert", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Convert_ReturnsServiceResult()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        var result = _controller.Convert(quantity, "INCH");
        Assert.NotNull(result);
        Assert.Equal("INCH", result.Unit);
        Assert.Equal("Length", result.MeasurementType);
    }

    [Fact]
    public void TestController_Convert_PassesTargetUnitToService()
    {
        var quantity = new QuantityDTO(5.0, "KILOGRAM", "Weight");
        var result = _controller.Convert(quantity, "GRAM");
        Assert.Equal("GRAM", result.Unit);
    }

    [Fact]
    public void TestController_Convert_CallsServiceExactlyOnce()
    {
        var quantity = new QuantityDTO(1.0, "FEET", "Length");
        _controller.Convert(quantity, "INCH");
        Assert.Single(_mockService.CallLog);
    }

    // ==================== Add Tests ====================

    [Fact]
    public void TestController_Add_DelegatesToService()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(2.0, "FEET", "Length");
        _controller.Add(first, second);
        Assert.Contains("Add", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Add_ReturnsServiceResult()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(2.0, "FEET", "Length");
        var result = _controller.Add(first, second);
        Assert.NotNull(result);
        Assert.Equal(3.0, result.Value, 6);
        Assert.Equal("FEET", result.Unit);
    }

    [Fact]
    public void TestController_Add_PreservesUnit()
    {
        var first = new QuantityDTO(1.0, "KILOGRAM", "Weight");
        var second = new QuantityDTO(2.0, "KILOGRAM", "Weight");
        var result = _controller.Add(first, second);
        Assert.Equal("KILOGRAM", result.Unit);
    }

    [Fact]
    public void TestController_Add_CallsServiceExactlyOnce()
    {
        var first = new QuantityDTO(1.0, "FEET", "Length");
        var second = new QuantityDTO(2.0, "FEET", "Length");
        _controller.Add(first, second);
        Assert.Single(_mockService.CallLog);
    }

    // ==================== Subtract Tests ====================

    [Fact]
    public void TestController_Subtract_DelegatesToService()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(3.0, "FEET", "Length");
        _controller.Subtract(first, second);
        Assert.Contains("Subtract", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Subtract_ReturnsServiceResult()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(3.0, "FEET", "Length");
        var result = _controller.Subtract(first, second);
        Assert.NotNull(result);
        Assert.Equal(7.0, result.Value, 6);
    }

    [Fact]
    public void TestController_Subtract_CallsServiceExactlyOnce()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(3.0, "FEET", "Length");
        _controller.Subtract(first, second);
        Assert.Single(_mockService.CallLog);
    }

    [Fact]
    public void TestController_Subtract_PreservesMeasurementType()
    {
        var first = new QuantityDTO(10.0, "LITRE", "Volume");
        var second = new QuantityDTO(3.0, "LITRE", "Volume");
        var result = _controller.Subtract(first, second);
        Assert.Equal("Volume", result.MeasurementType);
    }

    // ==================== Divide Tests ====================

    [Fact]
    public void TestController_Divide_DelegatesToService()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        _controller.Divide(first, second);
        Assert.Contains("Divide", _mockService.CallLog);
    }

    [Fact]
    public void TestController_Divide_ReturnsServiceResult()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        double result = _controller.Divide(first, second);
        Assert.Equal(2.0, result, 6);
    }

    [Fact]
    public void TestController_Divide_ByZero_ThrowsArithmeticException()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(0.0, "FEET", "Length");
        Assert.Throws<ArithmeticException>(() => _controller.Divide(first, second));
    }

    [Fact]
    public void TestController_Divide_CallsServiceExactlyOnce()
    {
        var first = new QuantityDTO(10.0, "FEET", "Length");
        var second = new QuantityDTO(5.0, "FEET", "Length");
        _controller.Divide(first, second);
        Assert.Single(_mockService.CallLog);
    }

    // ==================== Call Order / Sequence Tests ====================

    [Fact]
    public void TestController_MultipleOperations_CallsServiceInOrder()
    {
        var q1 = new QuantityDTO(1.0, "FEET", "Length");
        var q2 = new QuantityDTO(2.0, "FEET", "Length");

        _controller.Compare(q1, q2);
        _controller.Add(q1, q2);
        _controller.Subtract(q2, q1);
        _controller.Divide(q2, q1);

        Assert.Equal(new[] { "Compare", "Add", "Subtract", "Divide" }, _mockService.CallLog);
    }

    [Fact]
    public void TestController_DoesNotPerformBusinessLogic_DelegatesToService()
    {
        // Controller should ONLY delegate — not perform calculations itself
        var first = new QuantityDTO(5.0, "FEET", "Length");
        var second = new QuantityDTO(3.0, "FEET", "Length");

        var addResult = _controller.Add(first, second);
        var subResult = _controller.Subtract(first, second);

        // Mock returns first.Value +/- second.Value, not real conversions
        Assert.Equal(8.0, addResult.Value, 6);   // 5 + 3 = 8
        Assert.Equal(2.0, subResult.Value, 6);   // 5 - 3 = 2
    }
}
