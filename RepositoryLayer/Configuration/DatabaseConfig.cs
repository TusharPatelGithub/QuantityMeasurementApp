using Npgsql;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Configuration
{
    public static class DatabaseConfig
    {
        private static readonly string _connectionString;

        static DatabaseConfig()
        {
            // Walk up from the running assembly to find appsettings.json
            string basePath = AppContext.BaseDirectory;

            // Try to find appsettings.json walking up directories
            string? appSettingsPath = FindAppSettings(basePath);

            if (appSettingsPath != null)
            {
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(appSettingsPath)!)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                _connectionString = config.GetConnectionString("DefaultConnection")
                    ?? GetFallbackConnectionString();
            }
            else
            {
                _connectionString = GetFallbackConnectionString();
            }
        }

        private static string? FindAppSettings(string startPath)
        {
            var dir = new DirectoryInfo(startPath);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "appsettings.json");
                if (File.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }
            return null;
        }

        private static string GetFallbackConnectionString()
        {
            return "Host=localhost;Port=5432;Database=QuantityMeasurementDB;Username=postgres;Password=postgres";
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public static string GetConnectionString() => _connectionString;
    }
}
