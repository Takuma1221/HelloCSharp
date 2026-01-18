# EAVユーザー管理システム フォルダ構成

## 📁 プロジェクト構成（最新）

```
HelloCSharp/
│
├── 📂 Areas/UserManagement/          # ユーザー管理機能（EAVモデル）
│   ├── Controllers/
│   │   ├── AttributeController.cs    # View返却用（HTML返す）
│   │   ├── UserController.cs         # View返却用（HTML返す）
│   │   └── Api/
│   │       ├── AttributeSqlController.cs   # Web API（生SQL版・現在使用中）
│   │       ├── UserSqlController.cs        # Web API（生SQL版・現在使用中）
│   │       ├── UserAttributeValueController.cs  # Web API（属性値）
│   │       ├── AttributeApiController.cs   # Web API（EF Core版・参考用）
│   │       └── UserApiController.cs        # Web API（EF Core版・参考用）
│   │
│   ├── Models/
│   │   ├── User.cs                   # ユーザーエンティティ
│   │   ├── AttributeDefinition.cs    # 属性定義エンティティ
│   │   └── UserAttributeValue.cs     # ユーザー属性値エンティティ
│   │
│   ├── Repositories/                 # 【データアクセス層】
│   │   ├── IUserRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── IAttributeRepository.cs
│   │   ├── AttributeRepository.cs
│   │   ├── IUserAttributeValueRepository.cs
│   │   └── UserAttributeValueRepository.cs
│   │
│   ├── Services/                     # 【ビジネスロジック層】
│   │   ├── IUserService.cs
│   │   ├── UserService.cs
│   │   ├── IAttributeService.cs
│   │   ├── AttributeService.cs
│   │   ├── IUserAttributeValueService.cs
│   │   └── UserAttributeValueService.cs
│   │
│   └── Views/
│       ├── _ViewImports.cshtml
│       ├── _ViewStart.cshtml
│       ├── Attribute/
│       │   └── Index.cshtml          # → AttributePage.tsx
│       └── User/
│           └── Index.cshtml          # → UserPage.tsx
│
├── 📂 Controllers/                   # ルートコントローラー
│   └── HomeController.cs             # ホーム画面
│
├── 📂 Views/                         # ルートビュー
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   ├── Home/
│   │   └── Index.cshtml              # → HomePage.tsx
│   └── Shared/
│       ├── _Layout.cshtml            # 共通レイアウト
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml
│
├── 📂 Scripts/                       # TypeScript/Reactソース
│   └── react/
│       ├── pages/                    # 【ページ単位のエントリーポイント】
│       │   ├── HomePage.tsx          # ホーム画面（→ home-page.js）
│       │   └── AttributePage.tsx     # 属性管理画面（→ attribute-page.js）
│       │
│       ├── components/               # 【共通コンポーネント】
│       │   ├── AttributeTable.tsx    # 属性一覧テーブル
│       │   ├── AttributeModal.tsx    # 作成/編集モーダル
│       │   ├── Toast.tsx             # 通知コンポーネント
│       │   └── Loading.tsx           # ローディングオーバーレイ
│       │
│       └── shared/                   # 【共通モジュール】
│           ├── api.ts                # API呼び出し
│           └── types.ts              # TypeScript型定義
│
├── 📂 wwwroot/                       # 静的ファイル
│   ├── css/
│   │   └── site.css
│   └── js/
│       ├── site.js
│       └── react/
│           ├── home-page.js          # ビルド済み HomePage
│           └── attribute-page.js     # ビルド済み AttributePage
│
├── 📂 Data/                          # データベース関連
│   └── AppDbContext.cs               # EF Core DbContext（参考用）
│
├── 📂 Migrations/                    # EF Core マイグレーション
│
├── 📂 docs/                          # ドキュメント
│   ├── README.md
│   ├── architecture/
│   │   └── folder-structure.md       # このファイル
│   └── user-management/
│
├── Program.cs                        # エントリーポイント
├── HelloCSharp.csproj                # .NETプロジェクト設定
├── HelloCSharp.db                    # SQLiteデータベース
├── package.json                      # npm設定
├── tsconfig.json                     # TypeScript設定
└── README.md
```

---

## 🔗 cshtml ⇔ tsx ファイル対応表

| URL | cshtml | tsx (ページ) | js (出力) |
|-----|--------|-------------|-----------|
| `/` | `Views/Home/Index.cshtml` | `pages/HomePage.tsx` | `home-page.js` |
| `/UserManagement/Attribute` | `Areas/.../Views/Attribute/Index.cshtml` | `pages/AttributePage.tsx` | `attribute-page.js` |

### 命名規則

- **cshtml**: `{ページ名}/Index.cshtml` または `{ページ名}.cshtml`
- **tsx**: `pages/{ページ名}Page.tsx`
- **js出力**: `{ページ名(ケバブケース)}-page.js`

---

## 🔄 データフロー（React + Web API）

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              ブラウザ                                        │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │ Index.cshtml                                                           │ │
│  │   └─ <div id="react-root">                                            │ │
│  │   └─ <script src="/js/react/attribute-page.js">                       │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
│                              ↓                                              │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │ Scripts/react/                                                         │ │
│  │   pages/AttributePage.tsx                                              │ │
│  │     └→ components/*.tsx (UI部品)                                       │ │
│  │     └→ shared/api.ts → fetch('/api/UserManagement/AttributeSql')      │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
                              │ HTTP (JSON)
                              ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│                          ASP.NET Core                                        │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │ AttributeSqlController.cs（生SQL版）                                   │ │
│  │   [HttpGet]  GetAll()                                                  │ │
│  │   [HttpPost] Create()                                                  │ │
│  │   [HttpPut]  Update()                                                  │ │
│  │   [HttpDelete] Delete()                                                │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
│                              ↓                                              │
│  ┌───────────────────────────────────────────────────────────────────────┐ │
│  │ SqliteConnection → SQLite (HelloCSharp.db)                            │ │
│  └───────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 📂 Scripts/react/ の構成説明

```
Scripts/react/
├── pages/          # 各ページのエントリーポイント（1ページ1ファイル）
│                   # esbuildがここからバンドルを作成
│
├── components/     # 再利用可能なUIコンポーネント
│                   # 複数ページで使う部品を置く
│
└── shared/         # 共通ユーティリティ
                    # API呼び出し、型定義など
```

### 新しいページを追加する場合

1. `pages/{ページ名}Page.tsx` を作成
2. `Views/.../Index.cshtml` を作成（`<script src="/js/react/{ページ名(ケバブ)}-page.js">`）
3. `package.json` にビルドスクリプト追加
4. 必要に応じて `components/` に部品を追加

---

## 🛠️ ビルドコマンド

```bash
# 全ページビルド（開発用・sourcemap付き）
npm run build:dev

# 全ページビルド（本番用・minify）
npm run build

# 個別ビルド
npm run build:home:dev       # ホーム画面のみ
npm run build:attribute:dev  # 属性管理のみ

# ファイル監視
npm run watch:home           # ホーム画面
npm run watch:attribute      # 属性管理
```

---

## 🌐 URLマッピング

| URL | Controller | View | React |
|-----|------------|------|-------|
| `/` | HomeController.Index | Home/Index.cshtml | HomePage.tsx |
| `/UserManagement/Attribute` | AttributeController.Index | Attribute/Index.cshtml | AttributePage.tsx |
| `/api/UserManagement/AttributeSql` | AttributeSqlController | - | - |

---

## 📦 アーカイブ

学習初期のファイルは `../HelloCSharp_archive/` に移動済み：

```
HelloCSharp_archive/
├── Areas/Samples/           # 電卓・BMI（MVC学習サンプル）
├── Views/Attribute/         # 従来のMVC版View
├── Scripts/                 # 素のTypeScript版
├── wwwroot/js/              # 素のTS版JS
└── docs/                    # 学習初期のドキュメント
```
