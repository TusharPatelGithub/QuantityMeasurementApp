using ModelLayer.Models;
using BusinessLayer.Exceptions;
using QuantityMeasurementApp.Controllers;
using RepositoryLayer.DatabaseRepository;

namespace QuantityMeasurementApp;

public class Menu
{
    private readonly QuantityMeasurementController _controller;
    private readonly QuantityMeasurementDatabaseRepository _repository;

    public Menu(QuantityMeasurementController controller, QuantityMeasurementDatabaseRepository repository)
    {
        _controller = controller;
        _repository = repository;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("   Quantity Measurement Application");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
            Console.WriteLine("6. View All Measurements");
            Console.WriteLine("7. View By Measurement Type");
            Console.WriteLine("8. View By Operation Type");
            Console.WriteLine("9. Delete All Measurements");
            Console.WriteLine("0. Exit");
            Console.WriteLine("========================================");
            Console.Write("Enter your choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": DoCompare();    break;
                case "2": DoConvert();    break;
                case "3": DoAdd();        break;
                case "4": DoSubtract();   break;
                case "5": DoDivide();     break;
                case "6": ViewAll();      break;
                case "7": ViewByType();   break;
                case "8": ViewByOp();     break;
                case "9": DeleteAll();    break;
                case "0":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // -- Helpers ----------------------------------------------------------

    private static string AskMeasurementType()
    {
        Console.WriteLine("Measurement Type: 1=Length  2=Weight  3=Volume  4=Temperature");
        Console.Write("Enter choice: ");
        return Console.ReadLine() switch
        {
            "1" => "Length",
            "2" => "Weight",
            "3" => "Volume",
            "4" => "Temperature",
            _   => "Length"
        };
    }

    private static string AskUnit(string measurementType, string label)
    {
        Console.WriteLine($"{label} unit options:");
        switch (measurementType)
        {
            case "Length":
                Console.WriteLine("  FEET / INCH / YARDS / CENTIMETERS");
                break;
            case "Weight":
                Console.WriteLine("  KILOGRAM / GRAM / POUND");
                break;
            case "Volume":
                Console.WriteLine("  LITRE / MILLILITRE / GALLON");
                break;
            case "Temperature":
                Console.WriteLine("  CELSIUS / FAHRENHEIT");
                break;
        }
        Console.Write($"Enter {label} unit: ");
        return Console.ReadLine()?.ToUpper() ?? "";
    }

    private static double AskValue(string label)
    {
        Console.Write($"Enter {label} value: ");
        if (double.TryParse(Console.ReadLine(), out double val))
            return val;
        Console.WriteLine("Invalid value. Using 0.");
        return 0;
    }

    private static QuantityDTO AskQuantity(string label)
    {
        Console.WriteLine($"\n--- {label} ---");
        string type  = AskMeasurementType();
        double value = AskValue("quantity");
        string unit  = AskUnit(type, "quantity");
        return new QuantityDTO(value, unit, type);
    }

    // -- Operations -------------------------------------------------------

    private void DoCompare()
    {
        Console.WriteLine("\n=== COMPARE ===");
        var first  = AskQuantity("First Quantity");
        var second = AskQuantity("Second Quantity");
        try
        {
            bool result = _controller.Compare(first, second);
            Console.WriteLine($"\nResult: {first.Value} {first.Unit} == {second.Value} {second.Unit} ? {result}");
        }
        catch (QuantityMeasurementException ex)
        {
            Console.WriteLine($"Measurement Error [{ex.ErrorCode}]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void DoConvert()
    {
        Console.WriteLine("\n=== CONVERT ===");
        var quantity   = AskQuantity("Quantity to Convert");
        string target  = AskUnit(quantity.MeasurementType, "target");
        try
        {
            var result = _controller.Convert(quantity, target);
            Console.WriteLine($"\nResult: {quantity.Value} {quantity.Unit} = {result.Value} {result.Unit}");
        }
        catch (QuantityMeasurementException ex)
        {
            Console.WriteLine($"Measurement Error [{ex.ErrorCode}]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void DoAdd()
    {
        Console.WriteLine("\n=== ADD ===");
        var first  = AskQuantity("First Quantity");
        var second = AskQuantity("Second Quantity");
        try
        {
            var result = _controller.Add(first, second);
            Console.WriteLine($"\nResult: {first.Value} {first.Unit} + {second.Value} {second.Unit} = {result.Value} {result.Unit}");
        }
        catch (QuantityMeasurementException ex)
        {
            Console.WriteLine($"Measurement Error [{ex.ErrorCode}]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void DoSubtract()
    {
        Console.WriteLine("\n=== SUBTRACT ===");
        var first  = AskQuantity("First Quantity");
        var second = AskQuantity("Second Quantity");
        try
        {
            var result = _controller.Subtract(first, second);
            Console.WriteLine($"\nResult: {first.Value} {first.Unit} - {second.Value} {second.Unit} = {result.Value} {result.Unit}");
        }
        catch (QuantityMeasurementException ex)
        {
            Console.WriteLine($"Measurement Error [{ex.ErrorCode}]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void DoDivide()
    {
        Console.WriteLine("\n=== DIVIDE ===");
        var first  = AskQuantity("First Quantity");
        var second = AskQuantity("Second Quantity");
        try
        {
            double result = _controller.Divide(first, second);
            Console.WriteLine($"\nResult: {first.Value} {first.Unit} / {second.Value} {second.Unit} = {result}");
        }
        catch (QuantityMeasurementException ex)
        {
            Console.WriteLine($"Measurement Error [{ex.ErrorCode}]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void ViewAll()
    {
        Console.WriteLine("\n=== ALL MEASUREMENTS ===");
        var all = _repository.GetAllMeasurements();
        if (all.Count == 0)
        {
            Console.WriteLine("No measurements found.");
            return;
        }
        Console.WriteLine($"{"Id",-5} {"Type",-12} {"Operation",-10} {"Value1",-10} {"Value2",-10} {"Result",-10} {"Unit",-12} {"CreatedAt"}");
        Console.WriteLine(new string('-', 80));
        foreach (var m in all)
            Console.WriteLine($"{m.Id,-5} {m.MeasurementType,-12} {m.OperationType,-10} {m.Value1,-10} {m.Value2,-10} {m.Result,-10} {m.Unit,-12} {m.CreatedAt}");
        Console.WriteLine($"\nTotal: {all.Count} record(s)");
    }

    private void ViewByType()
    {
        Console.WriteLine("\n=== VIEW BY MEASUREMENT TYPE ===");
        string type = AskMeasurementType();
        var results = _repository.GetMeasurementsByType(type);
        if (results.Count == 0)
        {
            Console.WriteLine($"No measurements found for type: {type}");
            return;
        }
        Console.WriteLine($"\n--- {type} Measurements ---");
        Console.WriteLine($"{"Id",-5} {"Operation",-10} {"Value1",-10} {"Value2",-10} {"Result",-10} {"Unit",-12} {"CreatedAt"}");
        Console.WriteLine(new string('-', 75));
        foreach (var m in results)
            Console.WriteLine($"{m.Id,-5} {m.OperationType,-10} {m.Value1,-10} {m.Value2,-10} {m.Result,-10} {m.Unit,-12} {m.CreatedAt}");
        Console.WriteLine($"\nTotal: {results.Count} record(s)");
    }

    private void ViewByOp()
    {
        Console.WriteLine("\n=== VIEW BY OPERATION TYPE ===");
        Console.WriteLine("Operations: Compare / Convert / Add / Subtract / Divide");
        Console.Write("Enter operation: ");
        string op = Console.ReadLine() ?? "";
        var results = _repository.GetMeasurementsByOperation(op);
        if (results.Count == 0)
        {
            Console.WriteLine($"No measurements found for operation: {op}");
            return;
        }
        Console.WriteLine($"\n--- {op} Operations ---");
        Console.WriteLine($"{"Id",-5} {"Type",-12} {"Value1",-10} {"Value2",-10} {"Result",-10} {"Unit",-12} {"CreatedAt"}");
        Console.WriteLine(new string('-', 75));
        foreach (var m in results)
            Console.WriteLine($"{m.Id,-5} {m.MeasurementType,-12} {m.Value1,-10} {m.Value2,-10} {m.Result,-10} {m.Unit,-12} {m.CreatedAt}");
        Console.WriteLine($"\nTotal: {results.Count} record(s)");
    }

    private void DeleteAll()
    {
        Console.Write("\nAre you sure you want to delete ALL measurements? (yes/no): ");
        string? confirm = Console.ReadLine();
        if (confirm?.ToLower() == "yes")
        {
            _repository.DeleteAll();
            Console.WriteLine("All measurements deleted.");
        }
        else
        {
            Console.WriteLine("Delete cancelled.");
        }
    }
}


