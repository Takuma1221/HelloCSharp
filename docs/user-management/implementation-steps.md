# ユーザー管理システム実装手順 - Step by Step（EAVモデル）

このドキュメントでは、EAVモデルを使ったユーザー管理システムを段階的に実装していきます。

## 📋 前提条件

- .NET 9.0 SDK インストール済み
- プロジェクトがビルド可能な状態
- `docs/user-management/requirements.md`と`er-diagram.md`を読了

## 🎯 実装の全体フロー

```
Step 1: 環境準備（NuGetパッケージ）
  ↓
Step 2: Entity作成（User, Attribute, UserAttributeValue）
  ↓
Step 3: DbContext設定・マイグレーション
  ↓
Step 4: 属性管理CRUD実装
  ↓
Step 5: ユーザー管理CRUD実装（基本）
  ↓
Step 6: 動的フォーム実装（属性値入力）
  ↓
Step 7: ユーザー詳細画面（属性値表示）
  ↓
Step 8: UI改善・完成
```

---

## Step 1: 環境準備（10分）

### 1-1. NuGetパッケージのインストール

```bash
# プロジェクトルートで実行
cd /Users/aokitakuma/workspace/HelloCSharp

# Entity Framework Core SQLite（既にインストール済みの場合はスキップ）
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 1-2. EF Coreツールの確認

```bash
# グローバルツールとしてインストール（既にインストール済みの場合はスキップ）
dotnet tool install --global dotnet-ef

# バージョン確認
dotnet ef --version
```

### 1-3. Dataフォルダの確認

```bash
# Dataフォルダがなければ作成
mkdir -p Data
```

---

## Step 2: Entity作成（30分）

### 2-1. User エンティティ作成

**ファイル**: `Areas/UserManagement/Models/User.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// ユーザーエンティティ
/// </summary>
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

    // ナビゲーションプロパティ
    public ICollection<UserAttributeValue> AttributeValues { get; set; } = new List<UserAttributeValue>();
}
```

### 2-2. AttributeDefinition エンティティ作成

**ファイル**: `Areas/UserManagement/Models/AttributeDefinition.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// 属性定義エンティティ
/// </summary>
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

    // ナビゲーションプロパティ
    public ICollection<UserAttributeValue> UserAttributeValues { get; set; } = new List<UserAttributeValue>();
}
```

### 2-3. UserAttributeValue エンティティ作成

**ファイル**: `Areas/UserManagement/Models/UserAttributeValue.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// ユーザー属性値エンティティ
/// </summary>
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

---

## Step 3: DbContext設定・マイグレーション（20分）

### 3-1. AppDbContext作成

**ファイル**: `Data/AppDbContext.cs`

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
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Attributes テーブル設定
        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttributeName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DataType).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // UserAttributeValues テーブル設定
        modelBuilder.Entity<UserAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);

            // 複合ユニークインデックス
            entity.HasIndex(e => new { e.UserId, e.AttributeId }).IsUnique();

            // 外部キー設定
            entity.HasOne(e => e.User)
                .WithMany(u => u.AttributeValues)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Attribute)
                .WithMany(a => a.UserAttributeValues)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // シードデータ: 初期属性定義
        modelBuilder.Entity<AttributeDefinition>().HasData(
            new AttributeDefinition { Id = 1, AttributeName = "年齢", DataType = "Number", DisplayOrder = 1, IsRequired = false, CreatedAt = new DateTime(2025, 11, 8) },
            new AttributeDefinition { Id = 2, AttributeName = "部署", DataType = "Text", DisplayOrder = 2, IsRequired = true, CreatedAt = new DateTime(2025, 11, 8) },
            new AttributeDefinition { Id = 3, AttributeName = "役職", DataType = "Text", DisplayOrder = 3, IsRequired = false, CreatedAt = new DateTime(2025, 11, 8) },
            new AttributeDefinition { Id = 4, AttributeName = "入社日", DataType = "Date", DisplayOrder = 4, IsRequired = true, CreatedAt = new DateTime(2025, 11, 8) }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
        foreach (var entry in entries)
        {
            if (entry.Entity is User user) user.UpdatedAt = DateTime.Now;
            else if (entry.Entity is UserAttributeValue value) value.UpdatedAt = DateTime.Now;
        }
    }
}
```

### 3-2. Program.cs にDbContext登録

`Program.cs`の`builder.Services.AddControllersWithViews();`の直後に追加：

```csharp
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=HelloCSharp.db"));
```

using文も追加：
```csharp
using HelloCSharp.Data;
using Microsoft.EntityFrameworkCore;
```

### 3-3. マイグレーション実行

```bash
# マイグレーション作成
dotnet ef migrations add CreateUserManagementTables

# データベース作成・更新
dotnet ef database update

# ビルド確認
dotnet build
```

---

## Step 4: 属性管理CRUD実装（40分）

### 4-1. AttributeController作成

**ファイル**: `Areas/UserManagement/Controllers/AttributeController.cs`

```csharp
using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloCSharp.Areas.UserManagement.Controllers;

[Area("UserManagement")]
public class AttributeController : Controller
{
    private readonly AppDbContext _context;

    public AttributeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /UserManagement/Attribute
    public async Task<IActionResult> Index()
    {
        var attributes = await _context.Attributes
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
        return View(attributes);
    }

    // GET: /UserManagement/Attribute/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /UserManagement/Attribute/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AttributeDefinition attribute)
    {
        if (!ModelState.IsValid)
        {
            return View(attribute);
        }

        _context.Attributes.Add(attribute);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /UserManagement/Attribute/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var attribute = await _context.Attributes.FindAsync(id);
        if (attribute == null) return NotFound();

        return View(attribute);
    }

    // POST: /UserManagement/Attribute/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AttributeDefinition attribute)
    {
        if (id != attribute.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(attribute);
        }

        try
        {
            _context.Update(attribute);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AttributeExists(id)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /UserManagement/Attribute/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var attribute = await _context.Attributes
            .Include(a => a.UserAttributeValues)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attribute == null) return NotFound();

        return View(attribute);
    }

    // POST: /UserManagement/Attribute/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var attribute = await _context.Attributes.FindAsync(id);
        if (attribute != null)
        {
            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> AttributeExists(int id)
    {
        return await _context.Attributes.AnyAsync(e => e.Id == id);
    }
}
```

### 4-2. _ViewImports と _ViewStart

**ファイル**: `Areas/UserManagement/Views/_ViewImports.cshtml`

```razor
@using HelloCSharp
@using HelloCSharp.Areas.UserManagement.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

**ファイル**: `Areas/UserManagement/Views/_ViewStart.cshtml`

```razor
@{
    Layout = "_Layout";
}
```

### 4-3. Attribute/Index View

**ファイル**: `Areas/UserManagement/Views/Attribute/Index.cshtml`

```html
@model IEnumerable<AttributeDefinition>

@{
    ViewData["Title"] = "属性管理";
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h2>🏷️ @ViewData["Title"]</h2>
    <a asp-action="Create" class="btn btn-success">➕ 新規属性追加</a>
</div>

<hr />

@if (!Model.Any())
{
    <div class="alert alert-info">
        属性が定義されていません。<a asp-action="Create">新規追加</a>してください。
    </div>
}
else
{
    <table class="table table-striped">
        <thead>
            <tr>
                <th>ID</th>
                <th>属性名</th>
                <th>データ型</th>
                <th>必須</th>
                <th>表示順</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model)
            {
                <tr>
                    <td>@item.Id</td>
                    <td>@item.AttributeName</td>
                    <td>
                        @if (item.DataType == "Text") { <span class="badge bg-primary">文字列</span> }
                        else if (item.DataType == "Number") { <span class="badge bg-success">数値</span> }
                        else if (item.DataType == "Date") { <span class="badge bg-info">日付</span> }
                    </td>
                    <td>@(item.IsRequired ? "○" : "-")</td>
                    <td>@item.DisplayOrder</td>
                    <td>
                        <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-sm btn-outline-primary">編集</a>
                        <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-sm btn-outline-danger">削除</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

<div class="mt-3">
    <a asp-area="" asp-controller="Home" asp-action="Index" class="btn btn-secondary">ホームに戻る</a>
</div>
```

### 4-4. Attribute/Create View

**ファイル**: `Areas/UserManagement/Views/Attribute/Create.cshtml`

```html
@model AttributeDefinition

@{
    ViewData["Title"] = "新規属性作成";
}

<h2>➕ @ViewData["Title"]</h2>

<hr />

<div class="row">
    <div class="col-md-6">
        <form asp-action="Create" method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>

            <div class="form-group mb-3">
                <label asp-for="AttributeName" class="form-label"></label>
                <input asp-for="AttributeName" class="form-control" placeholder="例: 血液型" />
                <span asp-validation-for="AttributeName" class="text-danger"></span>
            </div>

            <div class="form-group mb-3">
                <label asp-for="DataType" class="form-label"></label>
                <select asp-for="DataType" class="form-control">
                    <option value="Text">文字列 (Text)</option>
                    <option value="Number">数値 (Number)</option>
                    <option value="Date">日付 (Date)</option>
                </select>
                <span asp-validation-for="DataType" class="text-danger"></span>
            </div>

            <div class="form-group mb-3">
                <label asp-for="DisplayOrder" class="form-label"></label>
                <input asp-for="DisplayOrder" class="form-control" type="number" value="999" />
                <span asp-validation-for="DisplayOrder" class="text-danger"></span>
            </div>

            <div class="form-check mb-3">
                <input asp-for="IsRequired" class="form-check-input" />
                <label asp-for="IsRequired" class="form-check-label"></label>
            </div>

            <div class="form-group">
                <button type="submit" class="btn btn-primary">作成</button>
                <a asp-action="Index" class="btn btn-secondary">キャンセル</a>
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @await Html.PartialAsync("_ValidationScriptsPartial")
}
```

### 4-5. Edit/Delete Viewsも同様に作成

Edit.cshtml と Delete.cshtml も作成してください（Create.cshtmlを参考に）。

---

## Step 5以降

`implementation-steps.md`の続きとして、ユーザー管理CRUD、動的フォーム実装を記載します。

詳細は実装しながら進めましょう！

---

次は `dotnet build` を実行して、エラーがないか確認してから、Step 5（ユーザー管理実装）に進みます。
