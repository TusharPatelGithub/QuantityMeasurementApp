using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTOs;

/// <summary>
/// Input DTO for REST API requests.
/// Encapsulates one or two quantities along with an optional target unit.
/// </summary>
public class QuantityInputDTO
{
    /// <summary>The first (or only) quantity — required for all operations.</summary>
    [Required(ErrorMessage = "First quantity is required.")]
    public QuantityDTO First { get; set; } = null!;

    /// <summary>The second quantity — required for Compare, Add, Subtract, Divide.</summary>
    public QuantityDTO? Second { get; set; }

    /// <summary>Target unit — required for Convert operation.</summary>
    public string? TargetUnit { get; set; }
}
