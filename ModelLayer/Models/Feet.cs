namespace ModelLayer.Models;

public class Feet
{
    private readonly double measurementValue;

    public Feet(double measurementValue) => this.measurementValue = measurementValue;
    public double MeasurementValue => this.measurementValue;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || this.GetType() != obj.GetType()) return false;
        return this.measurementValue.CompareTo(((Feet)obj).measurementValue) == 0;
    }
    public override int GetHashCode() => this.measurementValue.GetHashCode();
    public override string ToString()  => $"{this.measurementValue} ft";
}
