using BusinessLayer.Services;
using ModelLayer.Models;
using QuantityMeasurementApp.Controllers;

namespace QuantityMeasurementApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine(" Quantity Measurement Application UC15 ");
        Console.WriteLine("========================================");

        // Create Service
        IQuantityMeasurementService service = new QuantityMeasurementServiceImpl();

        // Inject Service into Controller
        var controller = new QuantityMeasurementController(service);

        // Example 1: Compare Length
        var q1 = new QuantityDTO(1, "FEET", "Length");
        var q2 = new QuantityDTO(12, "INCH", "Length");

        bool result = controller.Compare(q1, q2);

        Console.WriteLine($"Compare 1 ft and 12 inch → {result}");

        // Example 2: Convert Length
        var converted = controller.Convert(q1, "INCH");

        Console.WriteLine($"Convert 1 ft to inch → {converted.Value} {converted.Unit}");

        // Example 3: Add Length
        var addResult = controller.Add(q1, q2);

        Console.WriteLine($"Add 1 ft + 12 inch → {addResult.Value} {addResult.Unit}");

        Console.WriteLine("========================================");
        Console.WriteLine(" UC15 N-Tier Architecture Running ");
        Console.WriteLine("========================================");
    }
}