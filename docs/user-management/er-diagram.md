# ユーザー管理システム - ER図とデータベース設計（EAVモデル）

## 📊 ER図（Entity-Relationship Diagram）

```
┌─────────────────────────────┐
│         Users               │
├─────────────────────────────┤
│ PK  Id           INT        │
│     Name         NVARCHAR   │──┐
│     Email        NVARCHAR   │  │
│     CreatedAt    DATETIME   │  │
│     UpdatedAt    DATETIME   │  │
└─────────────────────────────┘  │
                                 │ 1
                                 │
                                 │
                                 │ N
                ┌────────────────┴──────────────────┐
                │   UserAttributeValues             │
                ├───────────────────────────────────┤
                │ PK  Id           INT              │
                │ FK  UserId       INT              │
                │ FK  AttributeId  INT              │
                │     Value        NVARCHAR(500)    │
                │     CreatedAt    DATETIME         │
                │     UpdatedAt    DATETIME         │
                └─────────────┬─────────────────────┘
                              │ N
                              │
                              │
                              │ 1
┌─────────────────────────────┴─┐
│         Attributes            │
├───────────────────────────────┤
│ PK  Id              INT       │
│     AttributeName   NVARCHAR  │
│     DataType        NVARCHAR  │
│     DisplayOrder    INT       │
│     IsRequired      BIT       │
│     CreatedAt       DATETIME  │
└───────────────────────────────┘

凡例:
PK  = Primary Key (主キー)
FK  = Foreign Key (外部キー)
1   = 一方(One)
N   = 多方(Many)
```

## 🗄️ テーブル定義

### 1. Users テーブル

| カラム名 | データ型 | NULL許可 | デフォルト値 | 制約 | 説明 |
|---------|---------|---------|------------|------|------|
| Id | INTEGER | NO | - | PRIMARY KEY, AUTOINCREMENT | 自動採番される一意のID |
| Name | TEXT | NO | - | - | ユーザー名（最大100文字） |
| Email | TEXT | NO | - | UNIQUE | メールアドレス（ユニーク制約） |
| CreatedAt | TEXT | NO | CURRENT_TIMESTAMP | - | レコード作成日時 |
| UpdatedAt | TEXT | NO | CURRENT_TIMESTAMP | - | レコード更新日時 |

**インデックス**:
```sql
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
```

### 2. Attributes テーブル

| カラム名 | データ型 | NULL許可 | デフォルト値 | 制約 | 説明 |
|---------|---------|---------|------------|------|------|
| Id | INTEGER | NO | - | PRIMARY KEY, AUTOINCREMENT | 自動採番される一意のID |
| AttributeName | TEXT | NO | - | - | 属性名（例: "年齢", "部署"） |
| DataType | TEXT | NO | - | CHECK(DataType IN ('Text','Number','Date')) | データ型 |
| DisplayOrder | INTEGER | NO | - | - | 表示順序（昇順ソート用） |
| IsRequired | INTEGER | NO | 0 | - | 必須フラグ（0=任意, 1=必須） |
| CreatedAt | TEXT | NO | CURRENT_TIMESTAMP | - | レコード作成日時 |

**インデックス**:
```sql
CREATE INDEX IX_Attributes_DisplayOrder ON Attributes(DisplayOrder);
```

### 3. UserAttributeValues テーブル

| カラム名 | データ型 | NULL許可 | デフォルト値 | 制約 | 説明 |
|---------|---------|---------|------------|------|------|
| Id | INTEGER | NO | - | PRIMARY KEY, AUTOINCREMENT | 自動採番される一意のID |
| UserId | INTEGER | NO | - | FOREIGN KEY → Users(Id) ON DELETE CASCADE | ユーザーID |
| AttributeId | INTEGER | NO | - | FOREIGN KEY → Attributes(Id) ON DELETE CASCADE | 属性ID |
| Value | TEXT | NO | - | - | 属性値（すべて文字列で保存） |
| CreatedAt | TEXT | NO | CURRENT_TIMESTAMP | - | レコード作成日時 |
| UpdatedAt | TEXT | NO | CURRENT_TIMESTAMP | - | レコード更新日時 |

**複合インデックス**:
```sql
CREATE UNIQUE INDEX IX_UserAttributeValues_UserId_AttributeId 
ON UserAttributeValues(UserId, AttributeId);
```
→ 同じユーザーに対して同じ属性は1つだけ

**外部キー制約**:
```sql
FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
FOREIGN KEY (AttributeId) REFERENCES Attributes(Id) ON DELETE CASCADE
```

## 📐 C# Entity クラス定義

### User.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "名前を入力してください")]
    [StringLength(100, ErrorMessage = "名前は100文字以内で入力してください")]
    [Display(Name = "名前")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "正しいメールアドレスを入力してください")]
    [Display(Name = "メールアドレス")]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ: このユーザーが持つ属性値のリスト
    public ICollection<UserAttributeValue> AttributeValues { get; set; } = new List<UserAttributeValue>();
}
```

### Attribute.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

public class AttributeDefinition
{
    public int Id { get; set; }

    [Required(ErrorMessage = "属性名を入力してください")]
    [StringLength(50, ErrorMessage = "属性名は50文字以内で入力してください")]
    [Display(Name = "属性名")]
    public string AttributeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "データ型を選択してください")]
    [Display(Name = "データ型")]
    public string DataType { get; set; } = "Text"; // Text, Number, Date

    [Required(ErrorMessage = "表示順を入力してください")]
    [Range(1, 999, ErrorMessage = "表示順は1以上999以下で入力してください")]
    [Display(Name = "表示順")]
    public int DisplayOrder { get; set; } = 1;

    [Display(Name = "必須")]
    public bool IsRequired { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ: この属性定義を使っている値のリスト
    public ICollection<UserAttributeValue> UserAttributeValues { get; set; } = new List<UserAttributeValue>();
}
```

### UserAttributeValue.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

public class UserAttributeValue
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int AttributeId { get; set; }

    [Required(ErrorMessage = "値を入力してください")]
    [StringLength(500, ErrorMessage = "値は500文字以内で入力してください")]
    public string Value { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ
    public User User { get; set; } = null!;
    public AttributeDefinition Attribute { get; set; } = null!;
}
```

## 🏗️ DbContext 設計

```csharp
using Microsoft.EntityFrameworkCore;
using HelloCSharp.Areas.UserManagement.Models;

namespace HelloCSharp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<AttributeDefinition> Attributes { get; set; }
    public DbSet<UserAttributeValue> UserAttributeValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users テーブル設定
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });

        // Attributes テーブル設定
        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AttributeName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DataType)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.DisplayOrder);
        });

        // UserAttributeValues テーブル設定
        modelBuilder.Entity<UserAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(500);

            // 複合ユニークインデックス: 同じユーザー×属性の組み合わせは1つだけ
            entity.HasIndex(e => new { e.UserId, e.AttributeId })
                .IsUnique();

            // 外部キー設定
            entity.HasOne(e => e.User)
                .WithMany(u => u.AttributeValues)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // ユーザー削除時に属性値も削除

            entity.HasOne(e => e.Attribute)
                .WithMany(a => a.UserAttributeValues)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade); // 属性削除時に属性値も削除
        });

        // シードデータ: 初期属性定義
        modelBuilder.Entity<AttributeDefinition>().HasData(
            new AttributeDefinition
            {
                Id = 1,
                AttributeName = "年齢",
                DataType = "Number",
                DisplayOrder = 1,
                IsRequired = false,
                CreatedAt = DateTime.Now
            },
            new AttributeDefinition
            {
                Id = 2,
                AttributeName = "部署",
                DataType = "Text",
                DisplayOrder = 2,
                IsRequired = true,
                CreatedAt = DateTime.Now
            },
            new AttributeDefinition
            {
                Id = 3,
                AttributeName = "役職",
                DataType = "Text",
                DisplayOrder = 3,
                IsRequired = false,
                CreatedAt = DateTime.Now
            },
            new AttributeDefinition
            {
                Id = 4,
                AttributeName = "入社日",
                DataType = "Date",
                DisplayOrder = 4,
                IsRequired = true,
                CreatedAt = DateTime.Now
            }
        );
    }

    // SaveChanges時に自動的にUpdatedAtを更新
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                user.UpdatedAt = DateTime.Now;
            }
            else if (entry.Entity is UserAttributeValue value)
            {
                value.UpdatedAt = DateTime.Now;
            }
        }
    }
}
```

## 💾 サンプルデータ

### データ例

**Users**:
| Id | Name | Email | CreatedAt |
|----|------|-------|-----------|
| 1 | 青木拓馬 | aoki@example.com | 2025-11-08 10:00:00 |
| 2 | 山田太郎 | yamada@example.com | 2025-11-08 10:05:00 |

**Attributes**:
| Id | AttributeName | DataType | DisplayOrder | IsRequired |
|----|--------------|----------|--------------|------------|
| 1 | 年齢 | Number | 1 | false |
| 2 | 部署 | Text | 2 | true |
| 3 | 役職 | Text | 3 | false |
| 4 | 入社日 | Date | 4 | true |

**UserAttributeValues**:
| Id | UserId | AttributeId | Value |
|----|--------|-------------|-------|
| 1 | 1 | 1 | 25 |
| 2 | 1 | 2 | 開発部 |
| 3 | 1 | 3 | エンジニア |
| 4 | 1 | 4 | 2023-04-01 |
| 5 | 2 | 1 | 30 |
| 6 | 2 | 2 | 営業部 |

## 🔍 典型的なクエリ例

### ユーザーとすべての属性値を取得

```csharp
var user = await _context.Users
    .Include(u => u.AttributeValues)
        .ThenInclude(av => av.Attribute)
    .FirstOrDefaultAsync(u => u.Id == userId);

// 表示順でソート
var sortedValues = user.AttributeValues
    .OrderBy(av => av.Attribute.DisplayOrder)
    .ToList();
```

### 特定の属性値を検索

```csharp
// "部署"が"開発部"のユーザーを検索
var devUsers = await _context.Users
    .Where(u => u.AttributeValues.Any(av => 
        av.Attribute.AttributeName == "部署" && 
        av.Value == "開発部"))
    .ToListAsync();
```

---

次のステップ: `implementation-steps.md`で実装手順を確認してください。
