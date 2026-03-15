using BusinessLayer.Services;
using ModelLayer.Models;

namespace QuantityMeasurementApp.Controllers;

public class QuantityMeasurementController
{
    private readonly IQuantityMeasurementService _service;

    public QuantityMeasurementController(IQuantityMeasurementService service)
    {
        _service = service;
    }

    public bool Compare(QuantityDTO first, QuantityDTO second)
    {
        return _service.Compare(first, second);
    }

    public QuantityDTO Convert(QuantityDTO quantity, string targetUnit)
    {
        return _service.Convert(quantity, targetUnit);
    }

    public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
    {
        return _service.Add(first, second);
    }

    public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
    {
        return _service.Subtract(first, second);
    }

    public double Divide(QuantityDTO first, QuantityDTO second)
    {
        return _service.Divide(first, second);
    }
}