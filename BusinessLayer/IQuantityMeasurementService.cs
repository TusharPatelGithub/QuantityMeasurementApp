using ModelLayer.Models;

namespace BusinessLayer.Services;

public interface IQuantityMeasurementService
{
    // Compare two quantities
    bool Compare(QuantityDTO first, QuantityDTO second);

    // Convert one quantity to another unit
    QuantityDTO Convert(QuantityDTO quantity, string targetUnit);

    // Add two quantities
    QuantityDTO Add(QuantityDTO first, QuantityDTO second);

    // Subtract two quantities
    QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);

    // Divide two quantities
    double Divide(QuantityDTO first, QuantityDTO second);
}