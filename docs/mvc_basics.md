# MVC 基本とコードリーディングガイド

## 1. MVC とは

- Model: ドメイン/データ構造。View へ渡す生データ or ViewModel。
- View: Razor テンプレート。動的 HTML 生成。
- Controller: 入力を受け、Model/サービスを利用し View/JSON/Redirect を返す。

## 2. このサンプルの流れ

1. ブラウザが `GET /` を要求。
2. ルート定義 (Program.cs) の default ルートにより `HomeController.Index` が呼ばれる。
3. Controller 内で `ViewData` を設定し `return View();`。
4. View Engine が `Views/Home/Index.cshtml` を探し `_ViewStart` -> `_Layout` を適用。
5. HTML を返却。

## 3. Razor の基本

```cshtml
@{ var now = DateTime.Now; }
<p>@now: 現在時刻</p>
@if(now.Hour < 12){ <span>午前</span> } else { <span>午後</span> }
```

- `@` で C# に切り替え。
- `@{ ... }` コードブロック。
- 式は `@式` で表示。

## 4. Tag Helpers 例

```cshtml
<form asp-controller="Home" asp-action="About" method="post"></form>
```

HTML に近い形でリッチなヘルパーが利用可能。

## 5. DI とサービス登録

```csharp
builder.Services.AddControllersWithViews(); // MVC関連サービス一括登録
```

カスタムサービス例:

```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

Controller でコンストラクタインジェクション。

## 6. Middleware 順序重要性

Program.cs:

1. 例外処理 / HSTS
2. HTTPS リダイレクト
3. StaticFiles
4. Routing
5. Authorization
6. Endpoint(MapControllerRoute)

## 7. ルーティング

```csharp
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");
```

URL パターンをテンプレートで表現。属性ルーティングも利用可能。

## 8. ViewModel パターン

ViewData/ViewBag ではなく、型安全な ViewModel を推奨:

```csharp
public record AboutViewModel(string Message, DateTime GeneratedAt);
return View(new AboutViewModel("学習中", DateTime.Now));
```

View: `@model AboutViewModel`。

## 9. 環境別設定

`app.Environment.IsDevelopment()` で分岐。
将来的に `appsettings.Development.json` などで設定管理。

## 10. 発展トピック

- フィルター (認証/例外/結果)
- Areas によるモジュール化
- Partial View / View Component
- キャッシュ (ResponseCache / MemoryCache / Distributed Cache)
- Health Checks / gRPC / Minimal API 連携

## 11. 次の演習アイデア

| 演習                           | 目的                                |
| ------------------------------ | ----------------------------------- |
| フォーム POST + バリデーション | ModelBinding/DataAnnotations を理解 |
| 簡単な ToDo リスト             | CRUD の流れ習得                     |
| InMemoryDbContext 導入         | EF Core の基本                      |
| ログレベル変更                 | Logging/Configuration 把握          |
| ユニットテスト (Controller)    | テスト容易性確認                    |

---

この資料は最小構成を起点に体系的学習へ進むためのロードマップです。
