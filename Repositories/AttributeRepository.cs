using HelloCSharp.Models;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Repositories;

/// <summary>
/// 属性リポジトリ（生SQL実装）
/// </summary>
public class AttributeRepository : IAttributeRepository
{
    private readonly string _connectionString;

    public AttributeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=HelloCSharp.db";
    }

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
                    attributes.Add(MapToAttribute(reader));
                }
            }
        }

        return attributes;
    }

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
                    return MapToAttribute(reader);
                }
            }
        }

        return null;
    }

    public async Task<AttributeDefinition> CreateAsync(AttributeDefinition attribute)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Attributes (AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt)
                VALUES (@name, @dataType, @displayOrder, @isRequired, @createdAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@name", attribute.AttributeName);
            command.Parameters.AddWithValue("@dataType", attribute.DataType);
            command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
            command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = (long)(await command.ExecuteScalarAsync() ?? 0L);
            attribute.Id = (int)newId;
            attribute.CreatedAt = DateTime.Now;
        }

        return attribute;
    }

    public async Task<AttributeDefinition> UpdateAsync(AttributeDefinition attribute)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Attributes
                SET AttributeName = @name, 
                    DataType = @dataType, 
                    DisplayOrder = @displayOrder, 
                    IsRequired = @isRequired
                WHERE Id = @id
            ";
            command.Parameters.AddWithValue("@id", attribute.Id);
            command.Parameters.AddWithValue("@name", attribute.AttributeName);
            command.Parameters.AddWithValue("@dataType", attribute.DataType);
            command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
            command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);

            await command.ExecuteNonQueryAsync();
        }

        return attribute;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Attributes WHERE Id = @id";
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
            command.CommandText = "SELECT COUNT(1) FROM Attributes WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            var count = (long)(await command.ExecuteScalarAsync() ?? 0L);
            return count > 0;
        }
    }

    public async Task<int> GetMaxDisplayOrderAsync()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(DisplayOrder), 0) FROM Attributes";

            var maxOrder = (long)(await command.ExecuteScalarAsync() ?? 0L);
            return (int)maxOrder;
        }
    }

    private static AttributeDefinition MapToAttribute(SqliteDataReader reader)
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
