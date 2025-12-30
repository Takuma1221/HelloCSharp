using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// 属性管理サービス（生SQL実装）
/// </summary>
public class AttributeService : IAttributeService
{
    private readonly string _connectionString;

    public AttributeService(IConfiguration configuration)
    {
        // 接続文字列を設定から取得（本番環境用）
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

    /// <summary>
    /// 全属性を取得
    /// </summary>
    public async Task<IEnumerable<AttributeDefinition>> GetAllAsync()
    {
        var attributes = new List<AttributeDefinition>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt
                FROM Attributes
                ORDER BY DisplayOrder
            ";

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    attributes.Add(new AttributeDefinition
                    {
                        Id = reader.GetInt32(0),
                        AttributeName = reader.GetString(1),
                        DataType = reader.GetString(2),
                        DisplayOrder = reader.GetInt32(3),
                        IsRequired = reader.GetBoolean(4),
                        CreatedAt = DateTime.Parse(reader.GetString(5))
                    });
                }
            }
        }

        return attributes;
    }

    /// <summary>
    /// IDで属性を取得
    /// </summary>
    public async Task<AttributeDefinition?> GetByIdAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt
                FROM Attributes
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new AttributeDefinition
                    {
                        Id = reader.GetInt32(0),
                        AttributeName = reader.GetString(1),
                        DataType = reader.GetString(2),
                        DisplayOrder = reader.GetInt32(3),
                        IsRequired = reader.GetBoolean(4),
                        CreatedAt = DateTime.Parse(reader.GetString(5))
                    };
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 属性を作成
    /// </summary>
    public async Task<AttributeDefinition> CreateAsync(AttributeDefinition attribute)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Attributes (AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt)
                VALUES (@attributeName, @dataType, @displayOrder, @isRequired, @createdAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@attributeName", attribute.AttributeName);
            command.Parameters.AddWithValue("@dataType", attribute.DataType);
            command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
            command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = (long)(await command.ExecuteScalarAsync() ?? 0);
            attribute.Id = (int)newId;
            attribute.CreatedAt = DateTime.Now;
        }

        return attribute;
    }

    /// <summary>
    /// 属性を更新
    /// </summary>
    public async Task<bool> UpdateAsync(AttributeDefinition attribute)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Attributes
                SET AttributeName = @attributeName,
                    DataType = @dataType,
                    DisplayOrder = @displayOrder,
                    IsRequired = @isRequired
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", attribute.Id);
            command.Parameters.AddWithValue("@attributeName", attribute.AttributeName);
            command.Parameters.AddWithValue("@dataType", attribute.DataType);
            command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
            command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// 属性を削除
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            // まず関連するUserAttributeValuesを削除
            var deleteValuesCommand = connection.CreateCommand();
            deleteValuesCommand.CommandText = "DELETE FROM UserAttributeValues WHERE AttributeId = @id";
            deleteValuesCommand.Parameters.AddWithValue("@id", id);
            await deleteValuesCommand.ExecuteNonQueryAsync();

            // 属性を削除
            var deleteAttributeCommand = connection.CreateCommand();
            deleteAttributeCommand.CommandText = "DELETE FROM Attributes WHERE Id = @id";
            deleteAttributeCommand.Parameters.AddWithValue("@id", id);

            var rowsAffected = await deleteAttributeCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// 属性名の重複チェック
    /// </summary>
    public async Task<bool> ExistsAsync(string attributeName, int? excludeId = null)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            if (excludeId.HasValue)
            {
                command.CommandText = @"
                    SELECT COUNT(*) FROM Attributes 
                    WHERE AttributeName = @attributeName AND Id != @excludeId
                ";
                command.Parameters.AddWithValue("@excludeId", excludeId.Value);
            }
            else
            {
                command.CommandText = @"
                    SELECT COUNT(*) FROM Attributes 
                    WHERE AttributeName = @attributeName
                ";
            }
            command.Parameters.AddWithValue("@attributeName", attributeName);

            var count = (long)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }
    }
}
