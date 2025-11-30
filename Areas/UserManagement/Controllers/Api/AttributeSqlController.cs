using HelloCSharp.Areas.UserManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

/// <summary>
/// 属性管理のWeb API Controller（生SQL版）
/// Entity Framework Core を使わず、ADO.NET で直接SQLを実行
/// </summary>
[Area("UserManagement")]
[ApiController]
[Route("api/[area]/[controller]")]
public class AttributeSqlController : ControllerBase
{
    // 接続文字列（本来は appsettings.json から取得すべき）
    private const string ConnectionString = "Data Source=HelloCSharp.db";

    /// <summary>
    /// 属性一覧を取得
    /// GET: /api/UserManagement/AttributeSql
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<AttributeDefinition>> GetAll()
    {
        var attributes = new List<AttributeDefinition>();

        // ① 接続を開く
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            // ② SQLコマンドを作成
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt
                    FROM Attributes
                    ORDER BY DisplayOrder
                ";

                // ③ SQLを実行して結果を読み取る
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // ④ 1行ずつオブジェクトに変換
                        var attr = new AttributeDefinition
                        {
                            Id = reader.GetInt32(0),              // 0番目のカラム: Id
                            AttributeName = reader.GetString(1),  // 1番目: AttributeName
                            DataType = reader.GetString(2),       // 2番目: DataType
                            DisplayOrder = reader.GetInt32(3),    // 3番目: DisplayOrder
                            IsRequired = reader.GetBoolean(4),    // 4番目: IsRequired
                            CreatedAt = reader.GetDateTime(5)     // 5番目: CreatedAt
                        };
                        attributes.Add(attr);
                    }
                }
            }
        }
        // ⑤ using を抜けると自動的に接続が閉じられる

        return Ok(attributes);
    }

    /// <summary>
    /// 特定の属性を取得
    /// GET: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<AttributeDefinition> GetById(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        
        // パラメータ化クエリ（SQLインジェクション対策）
        command.CommandText = @"
            SELECT Id, AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt
            FROM Attributes
            WHERE Id = @id
        ";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        
        if (!reader.Read())
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        var attr = new AttributeDefinition
        {
            Id = reader.GetInt32(0),
            AttributeName = reader.GetString(1),
            DataType = reader.GetString(2),
            DisplayOrder = reader.GetInt32(3),
            IsRequired = reader.GetBoolean(4),
            CreatedAt = reader.GetDateTime(5)
        };

        return Ok(attr);
    }

    /// <summary>
    /// 新規属性を作成
    /// POST: /api/UserManagement/AttributeSql
    /// </summary>
    [HttpPost]
    public ActionResult<AttributeDefinition> Create([FromBody] AttributeDefinition attribute)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        
        // INSERT して、作成された Id を返す
        command.CommandText = @"
            INSERT INTO Attributes (AttributeName, DataType, DisplayOrder, IsRequired, CreatedAt)
            VALUES (@name, @dataType, @displayOrder, @isRequired, @createdAt);
            SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("@name", attribute.AttributeName);
        command.Parameters.AddWithValue("@dataType", attribute.DataType);
        command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
        command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);

        // ExecuteScalar: 1つの値を返すクエリに使う
        var newId = Convert.ToInt32(command.ExecuteScalar());
        attribute.Id = newId;

        return CreatedAtAction(nameof(GetById), new { id = newId }, attribute);
    }

    /// <summary>
    /// 属性を更新
    /// PUT: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] AttributeDefinition attribute)
    {
        if (id != attribute.Id)
        {
            return BadRequest(new { message = "IDが一致しません" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Attributes
            SET AttributeName = @name,
                DataType = @dataType,
                DisplayOrder = @displayOrder,
                IsRequired = @isRequired
            WHERE Id = @id
        ";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", attribute.AttributeName);
        command.Parameters.AddWithValue("@dataType", attribute.DataType);
        command.Parameters.AddWithValue("@displayOrder", attribute.DisplayOrder);
        command.Parameters.AddWithValue("@isRequired", attribute.IsRequired);

        // ExecuteNonQuery: 更新/削除などの行数を返す
        var rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(attribute);
    }

    /// <summary>
    /// 属性を削除
    /// DELETE: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // まず使用中かチェック
        using (var checkCommand = connection.CreateCommand())
        {
            checkCommand.CommandText = @"
                SELECT COUNT(*) FROM UserAttributeValues WHERE AttributeId = @id
            ";
            checkCommand.Parameters.AddWithValue("@id", id);
            
            var usageCount = Convert.ToInt32(checkCommand.ExecuteScalar());
            if (usageCount > 0)
            {
                return BadRequest(new
                {
                    message = "この属性は使用中のため削除できません",
                    usageCount
                });
            }
        }

        // 削除実行
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Attributes WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return NoContent();
    }
}
