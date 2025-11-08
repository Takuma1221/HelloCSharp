# ASP.NET Core MVC 開発セットアップ

## 1. 必要条件

- .NET SDK (net9.0 対応) インストール済み
- VS Code 最新版

## 2. プロジェクト構成 (最小)

```
Controllers/HomeController.cs
Views/_ViewImports.cshtml
Views/_ViewStart.cshtml
Views/Shared/_Layout.cshtml
Views/Home/Index.cshtml
Views/Home/About.cshtml
wwwroot/css/site.css
wwwroot/js/site.js
Program.cs
HelloCSharp.csproj
```

## 3. ビルドと実行

開発用ローカル実行:

```bash
dotnet build
dotnet run
```

ホットリロード (既存 task `watch` 利用):

```bash
# VS Code のタスク: watch を実行
```

ブラウザで `https://localhost:5001` (または出力に表示された URL) を開く。

## 4. 推奨 VS Code 拡張機能

`.vscode/extensions.json` に記述済み:

- C# (ms-dotnettools.csharp)
- C# Dev Kit (ms-dotnettools.csdevkit)
- .NET Runtime Install Tool
- GitLens
- EditorConfig
- JS Debug Nightly (ブラウザ & Node デバッグ強化)
- Code Spell Checker (コメント/ドキュメント品質)

### 拡張機能プロファイル作成手順

1. コマンドパレット: "Profiles: Create Profile"
2. 名前例: `aspnet-mvc-learning`
3. 現在の設定・拡張をベースに作成
4. 不要拡張はプロファイルから削除し軽量化

## 5. 基本的な実装フロー

1. 要件整理 (表示したいページ/データ)
2. Model 定義 (必要なら `Models/` を作成)
3. Controller アクション追加 (例: `HomeController` に `Index` / `About`)
4. View 作成 (`Views/<Controller>/<Action>.cshtml`)
5. 共通レイアウト/部分ビューで再利用性向上
6. DI でサービス登録 (`builder.Services.Add...`)
7. 実行し動作確認 & 単体テスト追加

## 6. ASP.NET Core らしさ (学習ポイント)

- 統一的 DI コンテナ (ServiceCollection)
- Middleware パイプライン順序の重要性
- `appsettings.*.json` と 環境毎設定 (今回は簡略化)
- Razor + TagHelper による宣言的 UI
- ロギング/構成/ヘルスチェック
- Minimal hosting (Program.cs が簡潔)

## 7. 次のステップ候補

- Model + ViewModel + DataAnnotations バリデーション
- フォーム送信 (POST) と `AntiForgeryToken`
- 単体テスト (xUnit) 追加
- EF Core による永続化 (InMemory -> SQLite/SQL Server)
- ユーザー認証 (Identity) / 認可ポリシー
- Configuration/Options パターン
- Serilog 等で構造化ログ

## 8. トラブルシューティング

| 症状                | 対処                                              |
| ------------------- | ------------------------------------------------- |
| ビルド失敗          | `dotnet --version` を確認 / SDK 再インストール    |
| 静的ファイル 404    | `app.UseStaticFiles()` が Program.cs にあるか確認 |
| View が見つからない | パスとファイル名 (`Index.cshtml`) を再確認        |
| HTTPS 証明書警告    | 開発証明書 `dotnet dev-certs https --trust` 実行  |

## 9. 参考リンク

- https://learn.microsoft.com/aspnet/core/mvc
- https://learn.microsoft.com/dotnet/core

---

このドキュメントは学習用最小構成を前提。必要に応じて発展させてください。
