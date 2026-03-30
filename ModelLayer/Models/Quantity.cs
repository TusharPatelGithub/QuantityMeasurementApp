using ModelLayer.Interfaces;
using ModelLayer.Enums;

namespace ModelLayer.Models;

public class Quantity<U> where U : class, IMeasurable
{
    private readonly double measurementValue;
    private readonly U unit;

    public Quantity(double measurementValue, U unit)
    {
        if (double.IsNaN(measurementValue) || double.IsInfinity(measurementValue))
            throw new ArgumentException("Measurement value must be a finite number.");
        this.measurementValue = measurementValue;
        this.unit             = unit;
    }

    public double MeasurementValue => this.measurementValue;
    public U Unit                  => this.unit;

    public Quantity<U> ConvertTo(U targetUnit)
    {
        double baseValue     = this.unit.ConvertToBaseUnit(this.measurementValue);
        double converted     = Math.Round(targetUnit.ConvertFromBaseUnit(baseValue), 6);
        return new Quantity<U>(converted, targetUnit);
    }

    public static double Convert(double value, U sourceUnit, U targetUnit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Value must be a finite number.");
        return new Quantity<U>(value, sourceUnit).ConvertTo(targetUnit).MeasurementValue;
    }

    private void ValidateArithmeticOperands(Quantity<U> other, U? targetUnit, bool targetUnitRequired)
    {
        if (other is null)
            throw new ArgumentException("Cannot perform arithmetic with null measurement.");
        if (targetUnitRequired && targetUnit is null)
            throw new ArgumentException("Target unit cannot be null when explicitly specified.");
    }

    private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
    {
        ((IMeasurable)this.unit).ValidateOperationSupport(operation.ToString());
        double thisBase  = this.unit.ConvertToBaseUnit(this.measurementValue);
        double otherBase = other.unit.ConvertToBaseUnit(other.measurementValue);
        return operation.Compute(thisBase, otherBase);
    }

    public Quantity<U> Add(Quantity<U> other)
    {
        ValidateArithmeticOperands(other, null, false);
        double result = Math.Round(this.unit.ConvertFromBaseUnit(PerformBaseArithmetic(other, ArithmeticOperation.ADD)), 6);
        return new Quantity<U>(result, this.unit);
    }

    public Quantity<U> Add(Quantity<U> other, U targetUnit)
    {
        ValidateArithmeticOperands(other, targetUnit, true);
        double result = Math.Round(targetUnit.ConvertFromBaseUnit(PerformBaseArithmetic(other, ArithmeticOperation.ADD)), 6);
        return new Quantity<U>(result, targetUnit);
    }

    public static Quantity<U> Add(Quantity<U> first, Quantity<U> second)
    {
        if (first is null) throw new ArgumentException("First operand cannot be null.");
        return first.Add(second);
    }

    public static Quantity<U> Add(Quantity<U> first, Quantity<U> second, U targetUnit)
    {
        if (first is null) throw new ArgumentException("First operand cannot be null.");
        return first.Add(second, targetUnit);
    }

    public static Quantity<U> Add(double value1, U unit1, double value2, U unit2)
        => new Quantity<U>(value1, unit1).Add(new Quantity<U>(value2, unit2));

    public Quantity<U> Subtract(Quantity<U> other)
    {
        ValidateArithmeticOperands(other, null, false);
        double result = Math.Round(this.unit.ConvertFromBaseUnit(PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT)), 6);
        return new Quantity<U>(result, this.unit);
    }

    public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
    {
        ValidateArithmeticOperands(other, targetUnit, true);
        double result = Math.Round(targetUnit.ConvertFromBaseUnit(PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT)), 6);
        return new Quantity<U>(result, targetUnit);
    }

    public double Divide(Quantity<U> other)
    {
        ValidateArithmeticOperands(other, null, false);
        return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;
        if (this.GetType() != obj.GetType()) return false;
        Quantity<U> other = (Quantity<U>)obj;
        return this.unit.ConvertToBaseUnit(this.measurementValue)
                   .CompareTo(other.unit.ConvertToBaseUnit(other.measurementValue)) == 0;
    }

    public override int GetHashCode()
        => this.unit.ConvertToBaseUnit(this.measurementValue).GetHashCode();

    public override string ToString()
        => $"{this.measurementValue} {this.unit.GetUnitName()}";
}
