# SQL → ORM 学習ガイド

## 学習の進め方

このガイドでは、**生SQL → Dapper → Entity Framework Core** の順で学習します。

### なぜこの順序？
1. **生SQL**: SQLの基礎を理解し、データベース操作の本質を学ぶ
2. **Dapper**: SQLを書きつつC#オブジェクトにマッピングする中間的アプローチ
3. **Entity Framework Core**: ORMの利点を理解し、開発速度を上げる

---

## Phase 1: 生SQL（ADO.NET）

### テーブル作成

```sql
-- SQLiteで実行
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    CreatedAt TEXT NOT NULL
);

CREATE TABLE Attributes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AttributeName TEXT NOT NULL UNIQUE,
    DataType TEXT NOT NULL CHECK(DataType IN ('string', 'number', 'date'))
);

CREATE TABLE UserAttributeValues (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    AttributeId INTEGER NOT NULL,
    StringValue TEXT,
    NumberValue REAL,
    DateValue TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (AttributeId) REFERENCES Attributes(Id) ON DELETE CASCADE
);

-- 初期データ
INSERT INTO Attributes (AttributeName, DataType) VALUES 
    ('Department', 'string'),
    ('Salary', 'number'),
    ('JoinDate', 'date');
```

### C#でのSQL実行（ADO.NET）

```csharp
// Data/SqlUserRepository.cs
using System.Data;
using Microsoft.Data.Sqlite;

public class SqlUserRepository
{
    private readonly string _connectionString;

    public SqlUserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 全ユーザー取得
    public List<User> GetAll()
    {
        var users = new List<User>();
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
            SELECT Id, Name, Email, CreatedAt 
            FROM Users 
            ORDER BY Id";

        using var command = new SqliteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            });
        }

        return users;
    }

    // ID指定で取得
    public User? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
            SELECT Id, Name, Email, CreatedAt 
            FROM Users 
            WHERE Id = @Id";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                CreatedAt = DateTime.Parse(reader.GetString(3))
            };
        }

        return null;
    }

    // ユーザー追加
    public void Add(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
            INSERT INTO Users (Name, Email, CreatedAt) 
            VALUES (@Name, @Email, @CreatedAt)";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

        command.ExecuteNonQuery();
    }

    // ユーザー更新
    public void Update(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
            UPDATE Users 
            SET Name = @Name, Email = @Email 
            WHERE Id = @Id";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Id", user.Id);

        command.ExecuteNonQuery();
    }

    // ユーザー削除
    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = "DELETE FROM Users WHERE Id = @Id";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    // 属性値付きで取得（JOIN）
    public User? GetUserWithAttributes(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // ユーザー取得
        var user = GetById(userId);
        if (user == null) return null;

        // 属性値取得（JOIN）
        var sql = @"
            SELECT 
                uav.Id,
                uav.AttributeId,
                a.AttributeName,
                a.DataType,
                uav.StringValue,
                uav.NumberValue,
                uav.DateValue
            FROM UserAttributeValues uav
            INNER JOIN Attributes a ON uav.AttributeId = a.Id
            WHERE uav.UserId = @UserId";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        using var reader = command.ExecuteReader();

        user.UserAttributeValues = new List<UserAttributeValue>();

        while (reader.Read())
        {
            user.UserAttributeValues.Add(new UserAttributeValue
            {
                Id = reader.GetInt32(0),
                AttributeId = reader.GetInt32(1),
                Attribute = new AttributeDefinition
                {
                    Id = reader.GetInt32(1),
                    AttributeName = reader.GetString(2),
                    DataType = reader.GetString(3)
                },
                StringValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                NumberValue = reader.IsDBNull(5) ? null : reader.GetDouble(5),
                DateValue = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6))
            });
        }

        return user;
    }
}
```

### 生SQLのメリット・デメリット

#### ✅ メリット
- SQLの完全な制御が可能
- パフォーマンスチューニングしやすい
- 複雑なクエリを最適化できる
- ORMのオーバーヘッドがない

#### ❌ デメリット
- コード量が多い
- SQLインジェクションのリスク（パラメータ化必須）
- 型安全性がない（文字列でSQL記述）
- マッピングコードが冗長
- `using`や`connection.Open()`の管理が必要

---

## Phase 2: Dapper（軽量ORM）

### NuGetパッケージ追加

```bash
dotnet add package Dapper
dotnet add package Microsoft.Data.Sqlite
```

### Dapperでのリポジトリ実装

```csharp
// Data/DapperUserRepository.cs
using Dapper;
using Microsoft.Data.Sqlite;

public class DapperUserRepository : IUserRepository
{
    private readonly string _connectionString;

    public DapperUserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 全ユーザー取得
    public List<User> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = "SELECT * FROM Users ORDER BY Id";
        
        return connection.Query<User>(sql).ToList();
    }

    // ID指定で取得
    public User? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = "SELECT * FROM Users WHERE Id = @Id";
        
        return connection.QueryFirstOrDefault<User>(sql, new { Id = id });
    }

    // ユーザー追加
    public void Add(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = @"
            INSERT INTO Users (Name, Email, CreatedAt) 
            VALUES (@Name, @Email, @CreatedAt)";
        
        connection.Execute(sql, user);
    }

    // ユーザー更新
    public void Update(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = @"
            UPDATE Users 
            SET Name = @Name, Email = @Email 
            WHERE Id = @Id";
        
        connection.Execute(sql, user);
    }

    // ユーザー削除
    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        
        var sql = "DELETE FROM Users WHERE Id = @Id";
        
        connection.Execute(sql, new { Id = id });
    }

    // 属性値付きで取得（Multi-Mapping）
    public User? GetUserWithAttributes(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
            SELECT 
                u.*,
                uav.Id, uav.UserId, uav.AttributeId, 
                uav.StringValue, uav.NumberValue, uav.DateValue,
                a.Id, a.AttributeName, a.DataType
            FROM Users u
            LEFT JOIN UserAttributeValues uav ON u.Id = uav.UserId
            LEFT JOIN Attributes a ON uav.AttributeId = a.Id
            WHERE u.Id = @UserId";

        var userDict = new Dictionary<int, User>();

        connection.Query<User, UserAttributeValue, AttributeDefinition, User>(
            sql,
            (user, attrValue, attr) =>
            {
                if (!userDict.TryGetValue(user.Id, out var currentUser))
                {
                    currentUser = user;
                    currentUser.UserAttributeValues = new List<UserAttributeValue>();
                    userDict.Add(user.Id, currentUser);
                }

                if (attrValue != null && attr != null)
                {
                    attrValue.Attribute = attr;
                    currentUser.UserAttributeValues.Add(attrValue);
                }

                return currentUser;
            },
            new { UserId = userId },
            splitOn: "Id,Id"
        );

        return userDict.Values.FirstOrDefault();
    }
}
```

### Program.csでの登録

```csharp
// Dapperの場合
builder.Services.AddScoped<IUserRepository>(provider =>
    new DapperUserRepository("Data Source=HelloCSharp.db"));
```

### Dapperのメリット・デメリット

#### ✅ メリット
- **SQLを書ける**: 会社のスタイルと同じ
- **軽量**: Entity Framework Coreより高速
- **シンプル**: マッピングが自動
- **柔軟性**: 複雑なSQLも記述可能

#### ❌ デメリット
- マイグレーション機能なし（SQLを手動管理）
- 変更追跡なし（Update時に全カラム指定必要）
- リレーション読み込みは手動

---

## Phase 3: Entity Framework Core（フルORM）

### NuGetパッケージ追加

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef
```

### DbContext定義

```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<AttributeDefinition> Attributes => Set<AttributeDefinition>();
    public DbSet<UserAttributeValue> UserAttributeValues => Set<UserAttributeValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User設定
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Attribute設定
        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttributeName).IsRequired();
            entity.HasIndex(e => e.AttributeName).IsUnique();
        });

        // UserAttributeValue設定
        modelBuilder.Entity<UserAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserAttributeValues)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Attribute)
                .WithMany()
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 初期データ
        modelBuilder.Entity<AttributeDefinition>().HasData(
            new AttributeDefinition { Id = 1, AttributeName = "Department", DataType = "string" },
            new AttributeDefinition { Id = 2, AttributeName = "Salary", DataType = "number" },
            new AttributeDefinition { Id = 3, AttributeName = "JoinDate", DataType = "date" }
        );
    }
}
```

### EF Coreでのリポジトリ実装

```csharp
// Data/EfUserRepository.cs
public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<User> GetAll()
    {
        return _context.Users
            .Include(u => u.UserAttributeValues)
            .ThenInclude(uav => uav.Attribute)
            .ToList();
    }

    public User? GetById(int id)
    {
        return _context.Users
            .Include(u => u.UserAttributeValues)
            .ThenInclude(uav => uav.Attribute)
            .FirstOrDefault(u => u.Id == id);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
```

### マイグレーション実行

```bash
# マイグレーションファイル生成
dotnet ef migrations add CreateUserManagementTables

# データベース更新
dotnet ef database update
```

### Program.csでの登録

```csharp
// EF Coreの場合
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=HelloCSharp.db"));

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
```

### EF Coreのメリット・デメリット

#### ✅ メリット
- **マイグレーション機能**: スキーマ変更を自動管理
- **変更追跡**: 変更したプロパティだけUPDATE
- **LINQ**: タイプセーフなクエリ記述
- **Eager/Lazy Loading**: リレーションの自動読み込み
- **開発速度**: コード量が少ない

#### ❌ デメリット
- 学習コストが高い
- 複雑なクエリはSQLより遅い場合がある
- ブラックボックス化（生成されるSQLが見えにくい）

---

## 3つの手法の比較

| 項目 | 生SQL (ADO.NET) | Dapper | Entity Framework Core |
|------|----------------|--------|----------------------|
| **コード量** | 多い | 中 | 少ない |
| **パフォーマンス** | 最速 | 速い | 中（最適化可能） |
| **SQL記述** | 必須 | 必須 | LINQで記述可 |
| **型安全性** | なし | あり（クラスマッピング） | 完全（LINQ） |
| **マイグレーション** | 手動 | 手動 | 自動 |
| **変更追跡** | なし | なし | あり |
| **学習コスト** | 低（SQL知識必要） | 低～中 | 中～高 |
| **会社のスタイル** | ⭐⭐⭐ | ⭐⭐⭐ | ⭐ |

---

## 実装の推奨順序

### Step 1: 生SQLで基礎を学ぶ（Phase 1）
1. SQLiteでテーブル作成
2. `SqlUserRepository`を実装
3. CRUD操作を手動実装
4. JOIN文で属性値を取得

**学習ポイント**: SQLの基礎、パラメータ化、接続管理

---

### Step 2: Dapperで効率化（Phase 2）
1. Dapperパッケージ追加
2. `DapperUserRepository`に書き換え
3. マッピングコード削減を体感
4. Multi-Mappingでリレーション取得

**学習ポイント**: マイクロORM、会社のスタイルに近い実装

---

### Step 3: EF Coreで自動化（Phase 3）
1. EF Core パッケージ追加
2. `AppDbContext`作成
3. マイグレーション実行
4. `EfUserRepository`に書き換え

**学習ポイント**: フルORM、LINQ、変更追跡、マイグレーション

---

## まとめ

### 学習の流れ
1. **生SQL**: データベース操作の本質を理解
2. **Dapper**: SQLを書きつつ効率化
3. **EF Core**: 開発速度とメンテナンス性の向上

### 実務での使い分け
- **生SQL**: 超複雑なクエリ、パフォーマンス最優先
- **Dapper**: 会社のスタイル、SQLを書きたい、高速性が必要
- **EF Core**: スタートアップ、CRUD中心、開発速度優先

### 次のステップ
1. `docs/user-management/implementation-steps.md`を参照
2. Phase 1から順番に実装
3. 各手法のパフォーマンスを比較
4. 会社のコードでDapperやADO.NETを探してみる
