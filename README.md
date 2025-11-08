# HelloCSharp - ASP.NET Core MVC 学習プロジェクト

ASP.NET Core MVC の基礎を学ぶための実践的なサンプルプロジェクトです。

## 🎯 プロジェクト概要

このプロジェクトは、ASP.NET Core MVC の基本概念を実装を通じて学ぶために作成されました。

### 実装機能

1. **電卓アプリ** (`/Calculator`)
   - 四則演算（足し算、引き算、掛け算、割り算）
   - Data Annotations によるバリデーション
   - Model Binding の実践例

2. **BMI計算アプリ** (`/Bmi`)
   - 身長・体重からBMI計算
   - BMI判定（やせ型・普通・肥満）
   - ViewModel パターンの実装

## 🚀 開始方法

### 必要条件

- .NET SDK 9.0 以上
- VS Code（推奨）

### セットアップ

```bash
# リポジトリをクローン
git clone https://github.com/YOUR_USERNAME/HelloCSharp.git
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

`docs/` フォルダに詳細な学習ガイドがあります：

- **setup.md** - セットアップ手順とビルド方法
- **mvc-for-beginners.md** - MVC 初学者向けガイド
- **mvc_basics.md** - MVC 基礎とコードリーディング
- **calculator-app-explanation.md** - 電卓アプリの詳細解説
- **themes.md** - VS Code テーマ設定

## 🏗️ プロジェクト構造

```
HelloCSharp/
├── Controllers/          # コントローラー
│   ├── HomeController.cs
│   ├── CalculatorController.cs
│   └── BmiController.cs
├── Models/              # モデル・ViewModel
│   ├── Calculator/
│   └── Bmi/
├── Views/               # Razor ビュー
│   ├── Home/
│   ├── Calculator/
│   ├── Bmi/
│   └── Shared/
├── wwwroot/             # 静的ファイル
│   ├── css/
│   └── js/
├── docs/                # ドキュメント
└── Program.cs           # エントリーポイント
```

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
