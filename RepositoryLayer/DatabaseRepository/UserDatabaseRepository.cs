using Microsoft.Extensions.Configuration;
using System.Data;
using Npgsql;
using ModelLayer.Models;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.DatabaseRepository
{
    public class UserDatabaseRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserDatabaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection not found.");
        }

        public AppUser? GetUserByEmail(string email)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var cmd = new NpgsqlCommand(
                    "SELECT \"Id\", \"FullName\", \"Email\", \"PasswordHash\", \"MobileNumber\", \"GoogleId\" FROM \"Users\" WHERE \"Email\" = @email",
                    connection);
                cmd.Parameters.AddWithValue("@email", email);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AppUser
                        {
                            Id           = reader.GetInt32(0),
                            FullName     = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Email        = reader.GetString(2),
                            PasswordHash = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            MobileNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            GoogleId     = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                    }
                }
            }
            return null;
        }

        public AppUser? GetUserByGoogleId(string googleId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var cmd = new NpgsqlCommand(
                    "SELECT \"Id\", \"FullName\", \"Email\", \"PasswordHash\", \"MobileNumber\", \"GoogleId\" FROM \"Users\" WHERE \"GoogleId\" = @googleId",
                    connection);
                cmd.Parameters.AddWithValue("@googleId", googleId);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AppUser
                        {
                            Id           = reader.GetInt32(0),
                            FullName     = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Email        = reader.GetString(2),
                            PasswordHash = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            MobileNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            GoogleId     = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                    }
                }
            }
            return null;
        }

        public int CreateUser(AppUser user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                // PostgreSQL uses RETURNING instead of MSSQL's OUTPUT INSERTED.Id
                var cmd = new NpgsqlCommand(
                    "INSERT INTO \"Users\" (\"FullName\", \"Email\", \"PasswordHash\", \"MobileNumber\", \"GoogleId\") VALUES (@fullName, @email, @pwd, @mobileNumber, @googleId) RETURNING \"Id\"",
                    connection);

                cmd.Parameters.AddWithValue("@fullName",     user.FullName);
                cmd.Parameters.AddWithValue("@email",        user.Email);
                cmd.Parameters.AddWithValue("@pwd",          user.PasswordHash);
                cmd.Parameters.AddWithValue("@mobileNumber", user.MobileNumber);
                cmd.Parameters.AddWithValue("@googleId",     string.IsNullOrEmpty(user.GoogleId) ? (object)DBNull.Value : user.GoogleId);

                connection.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                return 0;
            }
        }
    }
}
