using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using ModelLayer.DTOs;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Controllers;

/// <summary>
/// REST Controller for Quantity Measurement operations.
/// Base route: /api/v1/quantities
/// </summary>
[ApiController]
[Route("api/v1/quantities")]
[Produces("application/json")]
public class QuantityMeasurementApiController : ControllerBase
{
    private readonly IQuantityMeasurementService _service;
    private readonly ILogger<QuantityMeasurementApiController> _logger;

    public QuantityMeasurementApiController(
        IQuantityMeasurementService service,
        ILogger<QuantityMeasurementApiController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    // ─── POST /api/v1/quantities/compare ──────────────────────────────────────

    /// <summary>Compare two quantities of the same measurement type.</summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/quantities/compare
    ///     {
    ///       "first":  { "value": 12, "unit": "INCH",  "measurementType": "Length" },
    ///       "second": { "value": 1,  "unit": "FEET",  "measurementType": "Length" }
    ///     }
    /// </remarks>
    [HttpPost("compare")]
    [ProducesResponseType(typeof(QuantityMeasurementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CompareQuantities([FromBody] QuantityInputDTO input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (input.First == null || input.Second == null)
            return BadRequest(new { error = "Both 'first' and 'second' quantities are required for compare." });

        _logger.LogInformation("POST /compare called.");
        var result = _service.Compare(input.First, input.Second);
        return Ok(result);
    }

    // ─── POST /api/v1/quantities/convert ──────────────────────────────────────

    /// <summary>Convert a quantity to a different unit.</summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/quantities/convert
    ///     {
    ///       "first":      { "value": 1, "unit": "FEET", "measurementType": "Length" },
    ///       "targetUnit": "INCH"
    ///     }
    /// </remarks>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(QuantityMeasurementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ConvertQuantity([FromBody] QuantityInputDTO input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (input.First == null)
            return BadRequest(new { error = "'first' quantity is required for convert." });
        if (string.IsNullOrWhiteSpace(input.TargetUnit))
            return BadRequest(new { error = "'targetUnit' is required for convert." });

        _logger.LogInformation("POST /convert called.");
        var result = _service.Convert(input.First, input.TargetUnit);
        return Ok(result);
    }

    // ─── POST /api/v1/quantities/add ──────────────────────────────────────────

    /// <summary>Add two quantities of the same measurement type.</summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(QuantityMeasurementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AddQuantities([FromBody] QuantityInputDTO input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (input.First == null || input.Second == null)
            return BadRequest(new { error = "Both 'first' and 'second' quantities are required for add." });

        _logger.LogInformation("POST /add called.");
        var result = _service.Add(input.First, input.Second);
        return Ok(result);
    }

    // ─── POST /api/v1/quantities/subtract ────────────────────────────────────

    /// <summary>Subtract second quantity from first quantity.</summary>
    [HttpPost("subtract")]
    [ProducesResponseType(typeof(QuantityMeasurementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SubtractQuantities([FromBody] QuantityInputDTO input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (input.First == null || input.Second == null)
            return BadRequest(new { error = "Both 'first' and 'second' quantities are required for subtract." });

        _logger.LogInformation("POST /subtract called.");
        var result = _service.Subtract(input.First, input.Second);
        return Ok(result);
    }

    // ─── POST /api/v1/quantities/divide ──────────────────────────────────────

    /// <summary>Divide first quantity by second quantity.</summary>
    [HttpPost("divide")]
    [ProducesResponseType(typeof(QuantityMeasurementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DivideQuantities([FromBody] QuantityInputDTO input)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (input.First == null || input.Second == null)
            return BadRequest(new { error = "Both 'first' and 'second' quantities are required for divide." });

        _logger.LogInformation("POST /divide called.");
        var result = _service.Divide(input.First, input.Second);
        return Ok(result);
    }

    // ─── GET /api/v1/quantities/history/{operation} ───────────────────────────

    /// <summary>Get history of measurements by operation type (e.g. COMPARE, ADD, CONVERT).</summary>
    [HttpGet("history/{operation}")]
    [ProducesResponseType(typeof(List<QuantityMeasurementDTO>), StatusCodes.Status200OK)]
    public IActionResult GetOperationHistory([FromRoute] string operation)
    {
        _logger.LogInformation("GET /history/{Operation} called.", operation);
        var result = _service.GetMeasurementsByOperation(operation.ToUpperInvariant());
        return Ok(result);
    }

    // ─── GET /api/v1/quantities/measurements/{type} ───────────────────────────

    /// <summary>Get all measurements by measurement type (e.g. Length, Weight, Volume, Temperature).</summary>
    [HttpGet("measurements/{type}")]
    [ProducesResponseType(typeof(List<QuantityMeasurementDTO>), StatusCodes.Status200OK)]
    public IActionResult GetMeasurementsByType([FromRoute] string type)
    {
        _logger.LogInformation("GET /measurements/{Type} called.", type);
        var result = _service.GetMeasurementsByType(type);
        return Ok(result);
    }

    // ─── GET /api/v1/quantities/count/{operation} ────────────────────────────

    /// <summary>Get count of successful operations by operation type.</summary>
    [HttpGet("count/{operation}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetOperationCount([FromRoute] string operation)
    {
        _logger.LogInformation("GET /count/{Operation} called.", operation);
        int count = _service.GetOperationCount(operation.ToUpperInvariant());
        return Ok(new { operation = operation.ToUpperInvariant(), count });
    }

    // ─── GET /api/v1/quantities/errors ───────────────────────────────────────

    /// <summary>Get all measurements that resulted in errors.</summary>
    [HttpGet("errors")]
    [ProducesResponseType(typeof(List<QuantityMeasurementDTO>), StatusCodes.Status200OK)]
    public IActionResult GetErrorMeasurements()
    {
        _logger.LogInformation("GET /errors called.");
        var result = _service.GetErrorMeasurements();
        return Ok(result);
    }
}
