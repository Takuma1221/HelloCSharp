# バックエンド構成図

## 📁 ディレクトリ構造

```
HelloCSharp/
├── Program.cs                          # エントリーポイント + DI設定
├── HelloCSharp.db                      # SQLite データベース
│
├── Controllers/
│   ├── HomeController.cs               # トップページ用
│   ├── AttributeController.cs          # 属性管理画面用
│   ├── UserController.cs               # ユーザー管理画面用
│   └── Api/
│       ├── AttributeSqlController.cs   # ✅ API（生SQL版）
│       ├── UserSqlController.cs        # ✅ API（生SQL版）
│       ├── UserAttributeValueController.cs  # ✅ API（属性値）
│       └── AttributeApiController.cs   # 📦 API（EF Core版・参考用）
│
├── Data/
│   └── AppDbContext.cs                 # EF Core DbContext（参考用）
│
├── Models/
│   ├── User.cs                         # ユーザーエンティティ
│   ├── AttributeDefinition.cs          # 属性定義エンティティ
│   └── UserAttributeValue.cs           # 属性値エンティティ
│
├── Repositories/                       # 【データアクセス層】
│   ├── IUserRepository.cs
│   ├── UserRepository.cs               # 生SQL実装
│   ├── IAttributeRepository.cs
│   ├── AttributeRepository.cs          # 生SQL実装
│   ├── IUserAttributeValueRepository.cs
│   └── UserAttributeValueRepository.cs  # 生SQL実装
│
├── Services/                           # 【ビジネスロジック層】
│   ├── IUserService.cs
│   ├── UserService.cs                  # バリデーション、重複チェック等
│   ├── IAttributeService.cs
│   ├── AttributeService.cs             # DisplayOrder管理等
│   ├── IUserAttributeValueService.cs
│   └── UserAttributeValueService.cs    # 一括保存処理
│
└── Views/
    ├── Home/Index.cshtml
    ├── Attribute/Index.cshtml          # React マウント用
    ├── User/Index.cshtml               # React マウント用
    └── Shared/_Layout.cshtml           # 共通レイアウト
```

---

## 🔄 リクエストフロー

```
                    ┌─────────────────────────────────────────────────────────┐
                    │                      ASP.NET Core MVC                    │
                    └─────────────────────────────────────────────────────────┘
                                              │
                    ┌─────────────────────────┴─────────────────────────┐
                    │                                                   │
                    ▼                                                   ▼
        ┌───────────────────────┐                         ┌───────────────────────┐
        │   View Controllers    │                         │    API Controllers    │
        │   (HTML を返す)        │                         │    (JSON を返す)       │
        └───────────────────────┘                         └───────────────────────┘
                    │                                                   │
        ┌───────────┴───────────┐                         ┌────────────┴────────────┐
        ▼                       ▼                         ▼                         ▼
┌───────────────┐    ┌───────────────────┐    ┌─────────────────────┐    ┌─────────────────┐
│HomeController │    │AttributeController│    │ UserSqlController   │    │AttributeApiController│
│ /             │    │/Attribute         │    │ AttributeSqlController│  │(参考用)          │
│               │    │UserController     │    │                     │    │                 │
│               │    │/User              │    │                     │    │                 │
└───────────────┘    └───────────────────┘    └─────────────────────┘    └─────────────────┘
        │                       │                         │
        ▼                       ▼                         ▼
┌───────────────┐    ┌───────────────────┐    ┌─────────────────────┐
│  Index.cshtml │    │  Index.cshtml     │    │  Service Layer      │
│  (HTML)       │    │  (React mount)    │    │  (ビジネスロジック)  │
└───────────────┘    └───────────────────┘    ├─────────────────────┤
                                               │ - UserService       │
                                               │ - AttributeService  │
                                               │ - UserAttributeValue│
                                               │   Service           │
                                               └─────────────────────┘
                                                          │
                                                          ▼
                                               ┌─────────────────────┐
                                               │ Repository Layer    │
                                               │ (データアクセス)     │
                                               ├─────────────────────┤
                                               │ - UserRepository    │
                                               │ - AttributeRepository│
                                               │ - UserAttributeValue│
                                               │   Repository        │
                                               │                     │
                                               │ ✅ 生SQL実装        │
                                               │ (Microsoft.Data.    │
                                               │  Sqlite)            │
                                               └─────────────────────┘
                                                          │
                                                          ▼
                                              ┌───────────────────────┐
                                              │   HelloCSharp.db      │
                                              │   (SQLite)            │
                                              ├───────────────────────┤
                                              │ - Users               │
                                              │ - Attributes          │
                                              │ - UserAttributeValues │
                                              └───────────────────────┘
```

---

## 🗂️ コントローラー一覧

| コントローラー | ルート | 役割 | データアクセス |
|--------------|--------|------|--------------|
| `HomeController` | `/` | トップページ表示 | なし |
| `AttributeController` | `/UserManagement/Attribute` | 属性管理画面（HTML） | なし |
| `AttributeSqlController` | `/api/UserManagement/AttributeSql` | 属性CRUD API | 生SQL |
| `AttributeApiController` | `/api/UserManagement/AttributeApi` | 属性CRUD API（参考） | EF Core |

---

## 🏷️ エンティティ関係

```
┌─────────────────┐       ┌─────────────────────────┐       ┌─────────────────────┐
│     Users       │       │   UserAttributeValues   │       │    Attributes       │
├─────────────────┤       ├─────────────────────────┤       ├─────────────────────┤
│ Id (PK)         │──┐    │ Id (PK)                 │    ┌──│ Id (PK)             │
│ Name            │  │    │ UserId (FK)             │────┘  │ AttributeName       │
│ Email           │  └───▶│ AttributeId (FK)        │◀──────│ DataType            │
│ CreatedAt       │       │ Value                   │       │ DisplayOrder        │
│ UpdatedAt       │       │ CreatedAt               │       │ IsRequired          │
└─────────────────┘       │ UpdatedAt               │       │ CreatedAt           │
                          └─────────────────────────┘       └─────────────────────┘
```

---

## 🏗️ アーキテクチャパターン

### レイヤー構成（リポジトリパターン）

```
Controller Layer (API)
        ↓
 Service Layer (ビジネスロジック)
        ↓
Repository Layer (データアクセス)
        ↓
    Database (SQLite)
```

### 各層の責務

**Controller層** (`*SqlController.cs`)
- HTTPリクエスト/レスポンスの処理
- JSON形式でのデータ返却
- ステータスコード管理（200, 404, 400等）

**Service層** (`*Service.cs`)
- ビジネスロジックの実装
- バリデーション（メール重複チェック、必須項目等）
- トランザクション制御
- 複数Repositoryの組み合わせ

**Repository層** (`*Repository.cs`)
- データベースアクセスのみに責任を限定
- 生SQL実行（パラメータ化クエリでSQLインジェクション対策）
- CRUD操作の実装

### 依存性注入（DI）

[Program.cs](../../Program.cs) にて設定：

```csharp
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();
builder.Services.AddScoped<IUserAttributeValueRepository, UserAttributeValueRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IUserAttributeValueService, UserAttributeValueService>();
```

---

## 📝 補足

- **生SQL版** (`*SqlController` + Repository) を現在使用中
- **EF Core版** (`*ApiController`) は学習・比較用として残存
- View用コントローラーはReactマウント用のHTMLを返すだけで、データ操作は行わない
- リポジトリパターンにより、テスタビリティとメンテナンス性が向上
