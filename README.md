# HelloCSharp - ASP.NET Core MVC 学習プロジェクト

ASP.NET Core MVC の基礎を学ぶための実践的なサンプルプロジェクトです。

## 🎯 プロジェクト概要

このプロジェクトは、ASP.NET Core MVC の基本概念を実装を通じて学ぶために作成されました。

### 実装機能

#### Areas/Samples（基礎学習サンプル）

1. **電卓アプリ** (`/Samples/Calculator`)
   - 四則演算（足し算、引き算、掛け算、割り算）
   - Data Annotations によるバリデーション
   - Model Binding の実践例

2. **BMI計算アプリ** (`/Samples/Bmi`)
   - 身長・体重からBMI計算
   - BMI判定（やせ型・普通・肥満）
   - ViewModel パターンの実装

#### Areas/TodoApp（データベース連携）

3. **ユーザー管理システム** (`/UserManagement/User`, `/UserManagement/Attribute`) - 実装予定
   - Entity Framework Core + SQLite
   - **EAVモデル**（Entity-Attribute-Value）
   - 動的スキーマ設計
   - 複雑なリレーション（1対多×2）
   - CRUD操作（作成・読取・更新・削除）
   - 非同期処理（async/await）

## 🚀 開始方法

### 必要条件

- .NET SDK 9.0 以上
- VS Code（推奨）

### セットアップ

```bash
# リポジトリをクローン
git clone https://github.com/Takuma1221/HelloCSharp.git
cd HelloCSharp

# 依存関係の復元
dotnet restore

# ビルド
dotnet build

# 実行
dotnet run
```

ブラウザで http://localhost:5000 にアクセス

## 📚 学習ドキュメント

### docs/samples（基礎編）

- **README.md** - サンプルアプリ全体のガイド
- **mvc-for-beginners.md** - MVC 初学者向けガイド
- **mvc_basics.md** - MVC 基礎とコードリーディング
- **calculator-app-explanation.md** - 電卓アプリの詳細解説

### docs/user-management（発展編）

- **requirements.md** - ユーザー管理システムの要件定義
- **er-diagram.md** - EAVモデルのデータベース設計とER図
- **implementation-steps.md** - 段階的な実装手順

### docs/（その他）

- **setup.md** - 初期セットアップ手順
- **themes.md** - VS Code テーマ設定

## 🏗️ プロジェクト構造

```
HelloCSharp/
├── Areas/                  # Area機能（機能別分離）
│   ├── Samples/           # 基礎学習サンプル
│   │   ├── Controllers/   # Calculator, Bmi
│   │   ├── Models/        # ViewModels
│   │   └── Views/         # Razor Views
│   └── UserManagement/    # ユーザー管理システム（EAVモデル）
│       ├── Controllers/   # User, Attribute
│       ├── Models/        # Entity, ViewModels
│       └── Views/         # CRUD Views
├── Controllers/           # ルートコントローラー
│   └── HomeController.cs  # Home, About
├── Data/                  # DbContext（DB設定）
│   └── AppDbContext.cs
├── Models/                # 共通モデル（必要に応じて）
├── Views/                 # ルートビュー
│   ├── Home/
│   └── Shared/           # _Layout など
├── wwwroot/              # 静的ファイル
│   ├── css/
│   └── js/
├── docs/                 # ドキュメント
│   ├── samples/          # 基礎編ドキュメント
│   └── user-management/  # ユーザー管理編ドキュメント
├── Migrations/           # EF Core マイグレーション
└── Program.cs            # エントリーポイント
```

## 🎓 学習の進め方

### Step 1: MVC基礎（完了）
1. `docs/samples/mvc-for-beginners.md` を読む
2. Calculator/Bmi アプリを動かす
3. `docs/samples/calculator-app-explanation.md` でコード理解

### Step 2: ユーザー管理システム実装（次のステップ）
1. `docs/user-management/requirements.md` で要件確認
2. `docs/user-management/er-diagram.md` でEAVモデル理解
3. `docs/user-management/implementation-steps.md` に従って実装

## 🎓 学習ポイント

- ✅ MVC パターン
- ✅ Model Binding
- ✅ Data Annotations（バリデーション）
- ✅ Razor 構文
- ✅ Tag Helpers
- ✅ Layout と Section
- ✅ ViewModel パターン
- ✅ POST/Redirect/GET パターン

## 🛠️ 推奨 VS Code 拡張機能

`.vscode/extensions.json` に推奨拡張リストがあります：

- C# (ms-dotnettools.csharp)
- C# Dev Kit (ms-dotnettools.csdevkit)
- EditorConfig
- GitLens

## 📖 参考資料

- [ASP.NET Core MVC 公式ドキュメント](https://learn.microsoft.com/aspnet/core/mvc)
- [Razor 構文リファレンス](https://learn.microsoft.com/aspnet/core/mvc/views/razor)

## 📝 ライセンス

MIT License

## 👤 作成者

学習用プロジェクト
