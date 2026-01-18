using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Repositories;

/// <summary>
/// ユーザー属性値リポジトリ（生SQL実装）
/// </summary>
public class UserAttributeValueRepository : IUserAttributeValueRepository
{
    private readonly string _connectionString;

    public UserAttributeValueRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

    public async Task<IEnumerable<UserAttributeValue>> GetByUserIdAsync(int userId)
    {
        var values = new List<UserAttributeValue>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AttributeId, Value, CreatedAt, UpdatedAt
                FROM UserAttributeValues
                WHERE UserId = @userId
            ";
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    values.Add(MapToUserAttributeValue(reader));
                }
            }
        }

        return values;
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM UserAttributeValues WHERE UserId = @userId";
            command.Parameters.AddWithValue("@userId", userId);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task CreateAsync(UserAttributeValue value)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO UserAttributeValues (UserId, AttributeId, Value, CreatedAt, UpdatedAt)
                VALUES (@userId, @attributeId, @value, @createdAt, @updatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@userId", value.UserId);
            command.Parameters.AddWithValue("@attributeId", value.AttributeId);
            command.Parameters.AddWithValue("@value", value.Value);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = (long)(await command.ExecuteScalarAsync() ?? 0L);
            value.Id = (int)newId;
            value.CreatedAt = DateTime.Now;
            value.UpdatedAt = DateTime.Now;
        }
    }

    public async Task CreateBatchAsync(IEnumerable<UserAttributeValue> values)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var value in values)
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO UserAttributeValues (UserId, AttributeId, Value, CreatedAt, UpdatedAt)
                            VALUES (@userId, @attributeId, @value, @createdAt, @updatedAt)
                        ";
                        command.Parameters.AddWithValue("@userId", value.UserId);
                        command.Parameters.AddWithValue("@attributeId", value.AttributeId);
                        command.Parameters.AddWithValue("@value", value.Value);
                        command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    private static UserAttributeValue MapToUserAttributeValue(SqliteDataReader reader)
    {
        return new UserAttributeValue
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            AttributeId = reader.GetInt32(2),
            Value = reader.GetString(3),
            CreatedAt = DateTime.Parse(reader.GetString(4)),
            UpdatedAt = DateTime.Parse(reader.GetString(5))
        };
    }
}
