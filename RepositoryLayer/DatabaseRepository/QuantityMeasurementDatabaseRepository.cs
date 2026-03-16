using ModelLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Configuration;

namespace RepositoryLayer.DatabaseRepository
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly ILogger<QuantityMeasurementDatabaseRepository> _logger;

        public QuantityMeasurementDatabaseRepository(
            ILogger<QuantityMeasurementDatabaseRepository>? logger = null)
        {
            _logger = logger ?? new Microsoft.Extensions.Logging.Abstractions.NullLogger<QuantityMeasurementDatabaseRepository>();
            _logger.LogInformation("QuantityMeasurementDatabaseRepository initialized. ConnectionString: {cs}",
                DatabaseConfig.GetConnectionString());
        }

        public void SaveMeasurement(QuantityMeasurementEntity entity)
        {
            _logger.LogInformation("Saving measurement: Type={Type}, Operation={Op}, Value1={V1}, Value2={V2}, Result={R}, Unit={U}",
                entity.MeasurementType, entity.OperationType,
                entity.Value1, entity.Value2, entity.Result, entity.Unit);

            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                string query = @"INSERT INTO Measurements
                                (MeasurementType, OperationType, Value1, Value2, Result, Unit, CreatedAt)
                                VALUES
                                (@MeasurementType, @OperationType, @Value1, @Value2, @Result, @Unit, @CreatedAt)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MeasurementType", entity.MeasurementType);
                command.Parameters.AddWithValue("@OperationType",   entity.OperationType);
                command.Parameters.AddWithValue("@Value1",          entity.Value1);
                command.Parameters.AddWithValue("@Value2",          entity.Value2);
                command.Parameters.AddWithValue("@Result",          entity.Result);
                command.Parameters.AddWithValue("@Unit",            entity.Unit);
                command.Parameters.AddWithValue("@CreatedAt",       entity.CreatedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }

            _logger.LogInformation("Measurement saved successfully.");
        }

        private static QuantityMeasurementEntity MapReader(SqlDataReader reader)
        {
            return new QuantityMeasurementEntity
            {
                Id              = (int)reader["Id"],
                MeasurementType = reader["MeasurementType"].ToString() ?? "",
                OperationType   = reader["OperationType"].ToString()   ?? "",
                Value1          = Convert.ToDouble(reader["Value1"]),
                Value2          = Convert.ToDouble(reader["Value2"]),
                Result          = Convert.ToDouble(reader["Result"]),
                Unit            = reader["Unit"].ToString()            ?? "",
                CreatedAt       = Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        public List<QuantityMeasurementEntity> GetAllMeasurements()
        {
            _logger.LogInformation("Retrieving all measurements.");
            var measurements = new List<QuantityMeasurementEntity>();
            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Measurements", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    measurements.Add(MapReader(reader));
            }
            _logger.LogInformation("Retrieved {Count} measurements.", measurements.Count);
            return measurements;
        }

        public int GetTotalCount()
        {
            _logger.LogInformation("Getting total measurement count.");
            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Measurements", connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                _logger.LogInformation("Total count: {Count}", count);
                return count;
            }
        }

        public void DeleteAll()
        {
            _logger.LogWarning("Deleting all measurements from database.");
            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                SqlCommand command = new SqlCommand("DELETE FROM Measurements", connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
            _logger.LogWarning("All measurements deleted.");
        }

        public List<QuantityMeasurementEntity> GetMeasurementsByType(string measurementType)
        {
            _logger.LogInformation("Retrieving measurements by type: {Type}", measurementType);
            var measurements = new List<QuantityMeasurementEntity>();
            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                SqlCommand command = new SqlCommand(
                    "SELECT * FROM Measurements WHERE MeasurementType = @MeasurementType", connection);
                command.Parameters.AddWithValue("@MeasurementType", measurementType);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    measurements.Add(MapReader(reader));
            }
            _logger.LogInformation("Retrieved {Count} measurements of type {Type}.", measurements.Count, measurementType);
            return measurements;
        }

        public List<QuantityMeasurementEntity> GetMeasurementsByOperation(string operationType)
        {
            _logger.LogInformation("Retrieving measurements by operation: {Op}", operationType);
            var measurements = new List<QuantityMeasurementEntity>();
            using (SqlConnection connection = DatabaseConfig.GetConnection())
            {
                SqlCommand command = new SqlCommand(
                    "SELECT * FROM Measurements WHERE OperationType = @OperationType", connection);
                command.Parameters.AddWithValue("@OperationType", operationType);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    measurements.Add(MapReader(reader));
            }
            _logger.LogInformation("Retrieved {Count} measurements of operation {Op}.", measurements.Count, operationType);
            return measurements;
        }
    }
}
