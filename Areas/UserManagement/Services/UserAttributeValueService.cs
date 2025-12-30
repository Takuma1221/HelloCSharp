using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー属性値サービス（生SQL実装）
/// </summary>
public class UserAttributeValueService : IUserAttributeValueService
{
    private readonly string _connectionString;

    public UserAttributeValueService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

    /// <summary>
    /// 特定ユーザーの全属性値を取得
    /// </summary>
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
                    values.Add(new UserAttributeValue
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        AttributeId = reader.GetInt32(2),
                        Value = reader.GetString(3),
                        CreatedAt = DateTime.Parse(reader.GetString(4)),
                        UpdatedAt = DateTime.Parse(reader.GetString(5))
                    });
                }
            }
        }

        return values;
    }

    /// <summary>
    /// ユーザーの属性値を一括保存（既存削除→新規挿入）
    /// </summary>
    public async Task SaveUserAttributesAsync(int userId, Dictionary<int, string> attributeValues)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 既存の属性値を全削除
                    var deleteCommand = connection.CreateCommand();
                    deleteCommand.Transaction = transaction;
                    deleteCommand.CommandText = "DELETE FROM UserAttributeValues WHERE UserId = @userId";
                    deleteCommand.Parameters.AddWithValue("@userId", userId);
                    await deleteCommand.ExecuteNonQueryAsync();

                    // 新しい属性値を挿入
                    foreach (var kvp in attributeValues)
                    {
                        // 空文字列はスキップ
                        if (string.IsNullOrWhiteSpace(kvp.Value))
                            continue;

                        var insertCommand = connection.CreateCommand();
                        insertCommand.Transaction = transaction;
                        insertCommand.CommandText = @"
                            INSERT INTO UserAttributeValues (UserId, AttributeId, Value, CreatedAt, UpdatedAt)
                            VALUES (@userId, @attributeId, @value, @createdAt, @updatedAt)
                        ";
                        insertCommand.Parameters.AddWithValue("@userId", userId);
                        insertCommand.Parameters.AddWithValue("@attributeId", kvp.Key);
                        insertCommand.Parameters.AddWithValue("@value", kvp.Value);
                        insertCommand.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        await insertCommand.ExecuteNonQueryAsync();
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

    /// <summary>
    /// 特定ユーザーの属性値を全削除
    /// </summary>
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
}
