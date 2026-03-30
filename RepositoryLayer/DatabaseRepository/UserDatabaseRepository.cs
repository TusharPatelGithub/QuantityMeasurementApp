using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
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
            using (var connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT Id, Email, PasswordHash, GoogleId FROM Users WHERE Email = @email", connection);
                cmd.Parameters.AddWithValue("@email", email);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AppUser
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            GoogleId = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };
                    }
                }
            }
            return null;
        }

        public AppUser? GetUserByGoogleId(string googleId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT Id, Email, PasswordHash, GoogleId FROM Users WHERE GoogleId = @googleId", connection);
                cmd.Parameters.AddWithValue("@googleId", googleId);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AppUser
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            GoogleId = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };
                    }
                }
            }
            return null;
        }

        public int CreateUser(AppUser user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(
                    "INSERT INTO Users (Email, PasswordHash, GoogleId) OUTPUT INSERTED.Id VALUES (@email, @pwd, @googleId)", 
                    connection);
                
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@pwd", user.PasswordHash);
                cmd.Parameters.AddWithValue("@googleId", string.IsNullOrEmpty(user.GoogleId) ? (object)DBNull.Value : user.GoogleId);

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
