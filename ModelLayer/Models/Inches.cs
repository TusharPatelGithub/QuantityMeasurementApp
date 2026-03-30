namespace ModelLayer.Models;

public class Inches
{
    private readonly double measurementValue;

    public Inches(double measurementValue) => this.measurementValue = measurementValue;
    public double MeasurementValue => this.measurementValue;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || this.GetType() != obj.GetType()) return false;
        return this.measurementValue.CompareTo(((Inches)obj).measurementValue) == 0;
    }
    public override int GetHashCode() => this.measurementValue.GetHashCode();
    public override string ToString()  => $"{this.measurementValue} inch";
}
