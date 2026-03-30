using ModelLayer.Interfaces;

namespace ModelLayer.Models;

public class QuantityModel<U> where U : class, IMeasurable
{
    public double Value { get; set; }
    public U Unit { get; set; }

    public QuantityModel(double value, U unit)
    {
        Value = value;
        Unit  = unit;
    }

    public Quantity<U> ToQuantity() => new Quantity<U>(Value, Unit);
}
