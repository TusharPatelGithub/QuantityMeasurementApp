using BusinessLayer.Services;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.Controllers;
using RepositoryLayer.DatabaseRepository;

namespace QuantityMeasurementApp;

class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Warning).AddConsole();
        });

        var repoLogger    = loggerFactory.CreateLogger<QuantityMeasurementDatabaseRepository>();
        var serviceLogger = loggerFactory.CreateLogger<QuantityMeasurementServiceImpl>();

        var repository  = new QuantityMeasurementDatabaseRepository(repoLogger);
        var service     = new QuantityMeasurementServiceImpl(repository, serviceLogger);
        var controller  = new QuantityMeasurementController(service);

        var menu = new Menu(controller, repository);
        menu.Run();
    }
}
