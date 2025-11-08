# ASP.NET Core MVC 初学者ガイド

このドキュメントでは、ASP.NET Core MVC の基本概念、データの流れ、プロジェクト構造を初学者向けに解説します。

## 目次
1. [MVCとは何か](#mvcとは何か)
2. [データの流れ（リクエストからレスポンスまで）](#データの流れ)
3. [プロジェクト構造の説明](#プロジェクト構造の説明)
4. [実際の例で見る処理の流れ](#実際の例で見る処理の流れ)
5. [よくある質問](#よくある質問)

---

## MVCとは何か

**MVC** は **Model-View-Controller** の略で、アプリケーションを3つの役割に分けて設計するパターンです。

### 各役割の説明

```
┌─────────────────────────────────────────────────┐
│                   ユーザー                        │
│            (ブラウザでアクセス)                    │
└──────────────┬──────────────────────────────────┘
               │ ① HTTPリクエスト
               ▼
┌──────────────────────────────────────────────────┐
│              Controller (コントローラー)           │
│  ・リクエストを受け取る                            │
│  ・必要なデータを準備（Modelを使う）                │
│  ・Viewにデータを渡す                              │
└──────────────┬───────────────────────────────────┘
               │ ② データ準備
               ▼
┌──────────────────────────────────────────────────┐
│                Model (モデル)                     │
│  ・データ構造（クラス）                            │
│  ・ビジネスロジック                                │
│  ・データベースとのやり取り                         │
└──────────────┬───────────────────────────────────┘
               │ ③ データを渡す
               ▼
┌──────────────────────────────────────────────────┐
│                 View (ビュー)                     │
│  ・HTMLを生成（Razorテンプレート）                 │
│  ・ユーザーに表示する内容                          │
└──────────────┬───────────────────────────────────┘
               │ ④ HTMLレスポンス
               ▼
┌──────────────────────────────────────────────────┐
│                   ユーザー                        │
│              (ブラウザで表示)                      │
└──────────────────────────────────────────────────┘
```

### 各部分の役割

| 部分 | 役割 | 例 |
|------|------|-----|
| **Model** | データとビジネスロジック | ユーザー情報、商品データ、計算処理 |
| **View** | 画面表示（HTML生成） | `.cshtml`ファイル（Razorテンプレート） |
| **Controller** | 処理の制御・橋渡し | `HomeController.cs`（リクエスト受付→処理→View選択） |

---

## データの流れ

実際にブラウザで `http://localhost:5000/Home/About` にアクセスした時の流れを追います。

### ステップバイステップ

```
1. ユーザーがURL入力: http://localhost:5000/Home/About
   ↓
2. ASP.NET Core のルーティング（Program.cs）が解析
   pattern: "{controller=Home}/{action=About}/{id?}"
   → Controller: Home, Action: About
   ↓
3. HomeController の About メソッドが呼ばれる
   ↓
4. Controller内でデータ準備
   ViewData["Message"] = "このアプリは学習用...";
   ↓
5. return View(); が実行される
   → Views/Home/About.cshtml を探す
   ↓
6. View Engine が Razor を処理
   - _ViewStart.cshtml を読み込み（Layout設定）
   - _Layout.cshtml を適用（共通レイアウト）
   - About.cshtml の内容を RenderBody() に埋め込み
   ↓
7. 最終的なHTMLを生成
   ↓
8. ブラウザに返却
   ↓
9. ユーザーが画面を見る
```

### コードで見る流れ

#### ① ルーティング設定（Program.cs）
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```
- `{controller=Home}`: デフォルトのコントローラーは Home
- `{action=Index}`: デフォルトのアクションは Index
- `{id?}`: オプションのパラメータ

#### ② Controller（Controllers/HomeController.cs）
```csharp
public class HomeController : Controller
{
    public IActionResult About()
    {
        // データを準備
        ViewData["Message"] = "このアプリは学習用の最小MVCサンプルです";
        
        // Viewを返す（Views/Home/About.cshtmlを探す）
        return View();
    }
}
```

#### ③ View（Views/Home/About.cshtml）
```cshtml
@{
    ViewData["Title"] = "About";
}
<h1>About</h1>
<p>@ViewData["Message"]</p>
```
- `@ViewData["Message"]` でControllerから渡されたデータを表示

#### ④ Layout（Views/Shared/_Layout.cshtml）
```cshtml
<!DOCTYPE html>
<html>
<head>
    <title>@ViewData["Title"] - HelloCSharp MVC</title>
</head>
<body>
    <main>
        @RenderBody() <!-- ここにAbout.cshtmlの内容が入る -->
    </main>
</body>
</html>
```

---

## プロジェクト構造の説明

このプロジェクトのファイル構成と各ファイルの役割を説明します。

```
HelloCSharp/
│
├── Controllers/              # コントローラー（処理の制御）
│   └── HomeController.cs     # Homeコントローラー（Index, About, Errorアクション）
│
├── Views/                    # ビュー（画面表示）
│   ├── _ViewImports.cshtml   # 全Viewで使う名前空間・TagHelper定義
│   ├── _ViewStart.cshtml     # 全Viewに適用される初期設定（Layout指定）
│   │
│   ├── Shared/               # 共通ビュー
│   │   ├── _Layout.cshtml    # 共通レイアウト（ヘッダー・フッター）
│   │   └── Error.cshtml      # エラー画面
│   │
│   └── Home/                 # Homeコントローラー用のビュー
│       ├── Index.cshtml      # Indexアクション用
│       └── About.cshtml      # Aboutアクション用
│
├── wwwroot/                  # 静的ファイル（CSS, JS, 画像など）
│   ├── css/
│   │   └── site.css          # サイト全体のスタイル
│   └── js/
│       └── site.js           # サイト全体のJavaScript
│
├── Models/                   # モデル（データ構造）※今回はまだ作成していない
│
├── Program.cs                # アプリケーションのエントリーポイント
│                              # サービス登録、ミドルウェア設定、ルーティング
│
├── HelloCSharp.csproj        # プロジェクト設定ファイル
│
└── docs/                     # ドキュメント
    ├── setup.md              # セットアップ手順
    ├── mvc_basics.md         # MVC基礎知識
    ├── themes.md             # テーマ設定
    └── mvc-for-beginners.md  # このファイル
```

### 重要ファイルの詳細

#### Program.cs
アプリケーション全体の設定を行う中心的なファイル。

```csharp
var builder = WebApplication.CreateBuilder(args);

// サービス登録（DIコンテナに追加）
builder.Services.AddControllersWithViews();  // MVCサービスを追加

var app = builder.Build();

// ミドルウェアパイプラインの設定（順序重要）
app.UseStaticFiles();      // wwwroot配下の静的ファイルを提供
app.UseRouting();          // ルーティング機能を有効化
app.UseAuthorization();    // 認証・認可（今回は未使用）

// ルート設定
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();  // アプリケーション起動
```

#### _ViewImports.cshtml
全てのViewで共通して使う設定を記述。

```cshtml
@using HelloCSharp
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
- `@using`: 名前空間のインポート
- `@addTagHelper`: Tag Helperを有効化（`<form asp-controller="...">`など）

#### _ViewStart.cshtml
全てのViewで最初に実行される処理。

```cshtml
@{
    Layout = "_Layout";  // 全ページで_Layout.cshtmlを使う
}
```

---

## 実際の例で見る処理の流れ

このプロジェクトで実際に動いている例を使って、データの流れを追ってみましょう。

### 例1: トップページ（/）にアクセス

#### URLとルーティング
```
URL: http://localhost:5000/
または http://localhost:5000/Home/Index
```

#### 処理の流れ
1. **ルーティング解析**
   - URLが `/` なので、デフォルトの `Home/Index` が選ばれる
   
2. **HomeController.Index() が呼ばれる**
   ```csharp
   public IActionResult Index()
   {
       ViewData["Message"] = "ASP.NET Core MVCへようこそ";
       return View();
   }
   ```

3. **Viewの検索**
   - `Views/Home/Index.cshtml` を探す
   
4. **Viewの処理**
   ```cshtml
   @{
       ViewData["Title"] = "Home";
   }
   <h1>@ViewData["Message"]</h1>
   <p>Views/Home/Index.cshtml を編集して学習を進めてください。</p>
   ```

5. **Layoutの適用**
   - `_ViewStart.cshtml` により `_Layout.cshtml` が適用される
   - `@RenderBody()` の部分に Index.cshtml の内容が埋め込まれる

6. **最終HTML生成**
   ```html
   <!DOCTYPE html>
   <html lang="ja">
   <head>
       <title>Home - HelloCSharp MVC</title>
       <link rel="stylesheet" href="/css/site.css" />
   </head>
   <body>
       <header>
           <nav>
               <a href="/">Home</a> |
               <a href="/Home/About">About</a>
           </nav>
       </header>
       <main class="content">
           <h1>ASP.NET Core MVCへようこそ</h1>
           <p>Views/Home/Index.cshtml を編集して学習を進めてください。</p>
       </main>
       <footer>...</footer>
   </body>
   </html>
   ```

### 例2: Aboutページ（/Home/About）にアクセス

#### データの受け渡し
```
Controller (準備)         View (表示)
    ↓                        ↓
ViewData["Message"]  →  @ViewData["Message"]
    ↓                        ↓
"このアプリは..."      表示: "このアプリは..."
```

---

## よくある質問

### Q1: ViewDataとは何ですか？
**A:** ControllerからViewにデータを渡すための辞書型のコンテナです。

```csharp
// Controller側
ViewData["Name"] = "太郎";
ViewData["Age"] = 25;

// View側
<p>名前: @ViewData["Name"]</p>
<p>年齢: @ViewData["Age"]</p>
```

**注意点:**
- 型安全ではない（stringキーでアクセス）
- より型安全な方法として **ViewModel** を使うのが推奨

### Q2: ViewModelとは何ですか？
**A:** Viewに渡すデータを専用のクラスにまとめたもの。型安全で推奨される方法です。

```csharp
// ViewModel定義
public class AboutViewModel
{
    public string Message { get; set; }
    public DateTime GeneratedAt { get; set; }
}

// Controller
public IActionResult About()
{
    var model = new AboutViewModel
    {
        Message = "このアプリは学習用です",
        GeneratedAt = DateTime.Now
    };
    return View(model);
}

// View
@model AboutViewModel
<h1>@Model.Message</h1>
<p>生成日時: @Model.GeneratedAt</p>
```

### Q3: Razorの @ とは何ですか？
**A:** C# コードをHTMLに埋め込むための記号です。

```cshtml
<!-- 変数の表示 -->
<p>@ViewData["Message"]</p>

<!-- 式の評価 -->
<p>今日は @DateTime.Now.ToString("yyyy年MM月dd日") です</p>

<!-- 制御構文 -->
@if (ViewData["IsAdmin"] != null)
{
    <p>管理者です</p>
}

<!-- リテラルの@を表示したい場合は@@ -->
<p>メールアドレス: user@@example.com</p>
```

### Q4: _Layout.cshtml の @RenderBody() は何ですか？
**A:** 各ページの内容が挿入される場所を示すプレースホルダーです。

```cshtml
<!-- _Layout.cshtml -->
<body>
    <header>共通ヘッダー</header>
    <main>
        @RenderBody() <!-- ここに各ページの内容が入る -->
    </main>
    <footer>共通フッター</footer>
</body>
```

### Q5: wwwroot フォルダの役割は？
**A:** 静的ファイル（CSS、JavaScript、画像など）を配置する場所です。

- `wwwroot/css/site.css` → ブラウザから `/css/site.css` でアクセス可能
- `wwwroot/js/site.js` → ブラウザから `/js/site.js` でアクセス可能
- `wwwroot/images/logo.png` → ブラウザから `/images/logo.png` でアクセス可能

**重要:** `Program.cs` で `app.UseStaticFiles();` を呼び出す必要があります。

### Q6: ルーティングのパターンを変更したい
**A:** `Program.cs` の `MapControllerRoute` で設定できます。

```csharp
// カスタムルート追加例
app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{year}/{month}/{day}",
    defaults: new { controller = "Blog", action = "Archive" });

// 複数ルート設定可能
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

---

## 次のステップ

このドキュメントでMVCの基本を理解したら、次は以下にチャレンジしましょう：

1. **ViewModelを使ってみる**
   - `Models/` フォルダを作成
   - 専用のViewModelクラスを定義
   - ViewDataの代わりに型安全なModelを使用

2. **フォーム送信（POST）を試す**
   - フォームから入力を受け取る
   - `[HttpPost]` 属性の使い方
   - データバリデーション

3. **データベース連携（EF Core）**
   - Entity Framework Core の導入
   - データベースへの保存・取得

4. **認証・認可**
   - ログイン機能
   - ユーザーごとの権限管理

5. **テストを書く**
   - 単体テスト（xUnit）
   - Controllerのテスト

---

## 参考資料

- [ASP.NET Core MVC 公式ドキュメント](https://learn.microsoft.com/aspnet/core/mvc)
- [Razor 構文リファレンス](https://learn.microsoft.com/aspnet/core/mvc/views/razor)
- このプロジェクトの他のドキュメント:
  - `docs/setup.md` - セットアップとビルド方法
  - `docs/mvc_basics.md` - MVC基礎とコードリーディング
  - `docs/themes.md` - テーマとUI設定

---

**学習のヒント:**
- まずは既存のコードを少しずつ変更して動きを確認
- エラーメッセージをよく読む（多くのヒントが含まれています）
- 小さく作って動かす→理解する→拡張する、のサイクルを繰り返す
- わからないことは公式ドキュメントで確認する習慣をつける

がんばってください！
