using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Repositories;

/// <summary>
/// ユーザーリポジトリ（生SQL実装）
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = new List<User>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Email, CreatedAt, UpdatedAt
                FROM Users
                ORDER BY CreatedAt DESC
            ";

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(MapToUser(reader));
                }
            }
        }

        return users;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Email, CreatedAt, UpdatedAt
                FROM Users
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return MapToUser(reader);
                }
            }
        }

        return null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Email, CreatedAt, UpdatedAt
                FROM Users
                WHERE Email = @email
            ";
            command.Parameters.AddWithValue("@email", email);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return MapToUser(reader);
                }
            }
        }

        return null;
    }

    public async Task<User> CreateAsync(User user)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Name, Email, CreatedAt, UpdatedAt)
                VALUES (@name, @email, @createdAt, @updatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = (long)(await command.ExecuteScalarAsync() ?? 0L);
            user.Id = (int)newId;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
        }

        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users
                SET Name = @name, Email = @email, UpdatedAt = @updatedAt
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
            user.UpdatedAt = DateTime.Now;
        }

        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            var count = (long)(await command.ExecuteScalarAsync() ?? 0L);
            return count > 0;
        }
    }

    private static User MapToUser(SqliteDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            CreatedAt = DateTime.Parse(reader.GetString(3)),
            UpdatedAt = DateTime.Parse(reader.GetString(4))
        };
    }
}
