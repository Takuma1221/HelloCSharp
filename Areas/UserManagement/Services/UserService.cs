using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー管理サービス（生SQL実装）
/// </summary>
public class UserService : IUserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

    /// <summary>
    /// 全ユーザーを取得
    /// </summary>
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
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        CreatedAt = DateTime.Parse(reader.GetString(3)),
                        UpdatedAt = DateTime.Parse(reader.GetString(4))
                    });
                }
            }
        }

        return users;
    }

    /// <summary>
    /// IDでユーザーを取得
    /// </summary>
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
        }

        return null;
    }

    /// <summary>
    /// ユーザーを作成
    /// </summary>
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

            var newId = (long)(await command.ExecuteScalarAsync() ?? 0);
            user.Id = (int)newId;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
        }

        return user;
    }

    /// <summary>
    /// ユーザーを更新
    /// </summary>
    public async Task<bool> UpdateAsync(User user)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users
                SET Name = @name,
                    Email = @email,
                    UpdatedAt = @updatedAt
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// ユーザーを削除
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            // まず関連するUserAttributeValuesを削除
            var deleteValuesCommand = connection.CreateCommand();
            deleteValuesCommand.CommandText = "DELETE FROM UserAttributeValues WHERE UserId = @id";
            deleteValuesCommand.Parameters.AddWithValue("@id", id);
            await deleteValuesCommand.ExecuteNonQueryAsync();

            // ユーザーを削除
            var deleteUserCommand = connection.CreateCommand();
            deleteUserCommand.CommandText = "DELETE FROM Users WHERE Id = @id";
            deleteUserCommand.Parameters.AddWithValue("@id", id);

            var rowsAffected = await deleteUserCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// メールアドレスの重複チェック
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            if (excludeId.HasValue)
            {
                command.CommandText = @"
                    SELECT COUNT(*) FROM Users 
                    WHERE Email = @email AND Id != @excludeId
                ";
                command.Parameters.AddWithValue("@excludeId", excludeId.Value);
            }
            else
            {
                command.CommandText = @"
                    SELECT COUNT(*) FROM Users 
                    WHERE Email = @email
                ";
            }
            command.Parameters.AddWithValue("@email", email);

            var count = (long)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }
    }
}
