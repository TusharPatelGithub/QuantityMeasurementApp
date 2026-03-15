using ModelLayer.Models;

namespace BusinessLayer.Services;

public class QuantityMeasurementService
{
// Service class responsible for performing quantity measurement operations
// Handles equality comparisons (UC1-UC4) and unit conversions (UC5)
// Compares two Feet measurement objects for equality (UC1 backward compatibility)
public bool CompareFeetMeasurements(Feet? firstMeasurement, Feet? secondMeasurement)
{
    if (firstMeasurement is null)
    {
        return false;
    }
    return firstMeasurement.Equals(secondMeasurement);
}

// Compares two Inches measurement objects for equality (UC2 backward compatibility)
public bool CompareInchesMeasurements(Inches? firstMeasurement, Inches? secondMeasurement)
{
    if (firstMeasurement is null)
    {
        return false;
    }
    return firstMeasurement.Equals(secondMeasurement);
}

// Compares two QuantityLength measurement objects for equality (UC3/UC4)
public bool CompareQuantityLengthMeasurements(QuantityLength? firstMeasurement, QuantityLength? secondMeasurement)
{
    if (firstMeasurement is null)
    {
        return false;
    }
    return firstMeasurement.Equals(secondMeasurement);
}

// Converts a value from one unit to another (UC5)
public double ConvertUnits(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
{
    return QuantityLength.Convert(value, sourceUnit, targetUnit);
}

// Adds two QuantityLength measurements (UC6)
public QuantityLength AddLengthMeasurements(QuantityLength first, QuantityLength second)
{
    return QuantityLength.Add(first, second);
}

// Adds two QuantityLength measurements with explicit target unit (UC7)
public QuantityLength AddLengthMeasurements(QuantityLength first, QuantityLength second, LengthUnit targetUnit)
{
    return QuantityLength.Add(first, second, targetUnit);
}

// Compares two QuantityWeight measurement objects for equality (UC9)
public bool CompareWeightMeasurements(QuantityWeight? firstMeasurement, QuantityWeight? secondMeasurement)
{
    if (firstMeasurement is null)
    {
        return false;
    }
    return firstMeasurement.Equals(secondMeasurement);
}

// Converts weight units (UC9)
public double ConvertWeightUnits(double value, WeightUnit sourceUnit, WeightUnit targetUnit)
{
    return QuantityWeight.Convert(value, sourceUnit, targetUnit);
}

// Adds two QuantityWeight measurements
public QuantityWeight AddWeightMeasurements(QuantityWeight first, QuantityWeight second)
{
    return QuantityWeight.Add(first, second);
}

// Adds two QuantityWeight measurements with explicit target unit
public QuantityWeight AddWeightMeasurements(QuantityWeight first, QuantityWeight second, WeightUnit targetUnit)
{
    return QuantityWeight.Add(first, second, targetUnit);
}

// UC10: Generic comparison method
public bool CompareQuantityMeasurements<U>(Quantity<U>? first, Quantity<U>? second) where U : class, IMeasurable
{
    if (first is null)
    {
        return false;
    }
    return first.Equals(second);
}

// UC10: Generic conversion method
public double ConvertQuantityUnits<U>(double value, U sourceUnit, U targetUnit) where U : class, IMeasurable
{
    return Quantity<U>.Convert(value, sourceUnit, targetUnit);
}

// UC10: Generic addition
public Quantity<U> AddQuantityMeasurements<U>(Quantity<U> first, Quantity<U> second) where U : class, IMeasurable
{
    return Quantity<U>.Add(first, second);
}

// UC10: Generic addition with target unit
public Quantity<U> AddQuantityMeasurements<U>(Quantity<U> first, Quantity<U> second, U targetUnit) where U : class, IMeasurable
{
    return Quantity<U>.Add(first, second, targetUnit);
}

// UC12: Generic subtraction
public Quantity<U> SubtractQuantityMeasurements<U>(Quantity<U> first, Quantity<U> second) where U : class, IMeasurable
{
    return first.Subtract(second);
}

// UC12: Generic subtraction with target unit
public Quantity<U> SubtractQuantityMeasurements<U>(Quantity<U> first, Quantity<U> second, U targetUnit) where U : class, IMeasurable
{
    return first.Subtract(second, targetUnit);
}

// UC12: Generic division
public double DivideQuantityMeasurements<U>(Quantity<U> first, Quantity<U> second) where U : class, IMeasurable
{
    return first.Divide(second);
}
}
