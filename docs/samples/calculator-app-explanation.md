# 電卓アプリの実装解説 - Data Annotations と Model Binding

このドキュメントでは、実装した電卓アプリで発生したエラーと、Data Annotations（データアノテーション）の仕組みを詳しく解説します。

## 目次
1. [Data Annotations の役割](#data-annotations-の役割)
2. [発生したエラーの詳細](#発生したエラーの詳細)
3. [電卓アプリのデータフロー](#電卓アプリのデータフロー)
4. [実装の各層の役割](#実装の各層の役割)

---

## Data Annotations の役割

### `[Display(Name = "1つ目の数値")]` はどこで使われるか

#### ViewModel 定義（Models/CalculatorInputViewModel.cs）
```csharp
public class CalculatorInputViewModel
{
    [Required(ErrorMessage = "1つ目の数値を入力してください")]
    [Display(Name = "1つ目の数値")]  // ← ここ
    public double? Number1 { get; set; }
}
```

#### View での使用（Views/Calculator/Index.cshtml）
```cshtml
<label asp-for="Number1"></label>
<!-- ↑ これが "1つ目の数値" というラベルになる -->

<input asp-for="Number1" class="form-control" />
<!-- ↑ バリデーションエラー時に "1つ目の数値を入力してください" と表示 -->
```

#### 生成されるHTML
```html
<label for="Number1">1つ目の数値</label>
<input id="Number1" name="Number1" type="number" class="form-control" />
```

### Data Annotations の種類と役割

| 属性 | 役割 | 使用例 |
|------|------|--------|
| `[Required]` | 必須入力チェック | フォーム送信時に空でないことを検証 |
| `[Display(Name="...")]` | 表示名を指定 | `<label asp-for>` で自動的にこの名前が使われる |
| `[Range(min, max)]` | 数値範囲チェック | 1〜100の範囲など |
| `[StringLength(max)]` | 文字列長チェック | 最大50文字など |
| `[EmailAddress]` | メール形式チェック | example@domain.com 形式 |
| `[RegularExpression("...")]` | 正規表現チェック | 電話番号や郵便番号など |

### Tag Helpers の自動生成

`asp-for` Tag Helper は Data Annotations を読み取って、自動的に以下を生成します：

1. **ラベルテキスト**: `[Display(Name)]` から取得
2. **バリデーションメッセージ**: `[Required(ErrorMessage)]` から取得
3. **HTML属性**: `type="number"`, `required` など
4. **クライアント側バリデーション**: jQuery Validation の属性

---

## 発生したエラーの詳細

### エラーメッセージ
```
System.InvalidOperationException: 
The following sections have been defined but have not been rendered by the page 
at '/Views/Shared/_Layout.cshtml': 'Scripts'. 
To ignore an unrendered section call IgnoreSection("sectionName").
```

### エラーの原因

#### 問題のあったコード構成

**Calculator/Index.cshtml（電卓のビュー）**
```cshtml
@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```
→ 「Scripts」という名前のセクションを**定義**している

**_Layout.cshtml（共通レイアウト）- 修正前**
```cshtml
<body>
    <main>@RenderBody()</main>
    <script src="/js/site.js"></script>
</body>
```
→ 「Scripts」セクションを**レンダリング（表示）していない**

### なぜエラーになるのか

Razorのセクション機構では：
1. 個別ページ（Index.cshtml）が `@section Scripts { ... }` でセクションを定義
2. レイアウト（_Layout.cshtml）が `@RenderSection("Scripts")` でセクションを表示

**定義したセクションが表示されない = エラー** となります。  
理由: 「定義したのに使わないのはおかしい」という安全機構。

### 解決方法

**_Layout.cshtml に追加**
```cshtml
<body>
    <main>@RenderBody()</main>
    <script src="/js/site.js"></script>
    @RenderSection("Scripts", required: false)
    <!-- ↑ これを追加 -->
</body>
```

`required: false` の意味：
- セクションがあれば表示する
- なければスキップ（エラーにしない）
- これで、セクションを定義していないページ（Home/Index.cshtml など）でもエラーにならない

### セクションの使い分け

| ページ | Scripts セクション | 用途 |
|--------|-------------------|------|
| Calculator/Index.cshtml | **あり** | jQuery Validation（バリデーション用） |
| Home/Index.cshtml | なし | 特別なスクリプト不要 |
| Home/About.cshtml | なし | 特別なスクリプト不要 |

---

## 電卓アプリのデータフロー

### 全体の流れ（図解）

```
┌─────────────────────────────────────────────────┐
│  1. ユーザーがフォームに入力                       │
│     - Number1: 10                                │
│     - Operation: add (足し算)                    │
│     - Number2: 5                                 │
└──────────────┬──────────────────────────────────┘
               │ POST /Calculator
               ▼
┌──────────────────────────────────────────────────┐
│  2. Model Binding (ASP.NET Core が自動実行)       │
│     HTTPリクエストから ViewModel を自動生成        │
│                                                  │
│     var input = new CalculatorInputViewModel     │
│     {                                            │
│         Number1 = 10,                            │
│         Number2 = 5,                             │
│         Operation = "add"                        │
│     };                                           │
└──────────────┬──────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────┐
│  3. バリデーション実行                            │
│     Data Annotations に基づいて検証               │
│                                                  │
│     if (!ModelState.IsValid)                     │
│     {                                            │
│         return View(input); // エラー再表示      │
│     }                                            │
└──────────────┬──────────────────────────────────┘
               │ バリデーションOK
               ▼
┌──────────────────────────────────────────────────┐
│  4. Controller が計算ロジックを実行               │
│     Calculate(10, 5, "add") → 15                 │
└──────────────┬──────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────┐
│  5. 結果 ViewModel を作成                         │
│     var resultViewModel = new                    │
│     CalculatorResultViewModel                    │
│     {                                            │
│         Number1 = 10,                            │
│         Number2 = 5,                             │
│         Operation = "add",                       │
│         Result = 15,                             │
│         Expression = "10 + 5 = 15"               │
│     };                                           │
└──────────────┬──────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────┐
│  6. Result ビューにデータを渡す                    │
│     return View("Result", resultViewModel);      │
└──────────────┬──────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────┐
│  7. Result.cshtml で結果を表示                    │
│     @Model.Expression  → "10 + 5 = 15"           │
│     @Model.Result      → 15                      │
└──────────────┬──────────────────────────────────┘
               │ HTML生成
               ▼
┌──────────────────────────────────────────────────┐
│  8. ユーザーのブラウザに表示                       │
└──────────────────────────────────────────────────┘
```

---

## 実装の各層の役割

### 1. Model/ViewModel 層

**CalculatorInputViewModel.cs** - 入力データの定義
```csharp
public class CalculatorInputViewModel
{
    [Required(ErrorMessage = "1つ目の数値を入力してください")]
    [Display(Name = "1つ目の数値")]
    public double? Number1 { get; set; }
    
    [Required(ErrorMessage = "2つ目の数値を入力してください")]
    [Display(Name = "2つ目の数値")]
    public double? Number2 { get; set; }
    
    [Required(ErrorMessage = "演算子を選択してください")]
    [Display(Name = "演算")]
    public string? Operation { get; set; }
}
```

**役割:**
- フォームからのデータ受け取り構造を定義
- バリデーションルールを宣言的に記述
- 型安全性を提供（double? は nullable double）

**CalculatorResultViewModel.cs** - 結果データの定義
```csharp
public class CalculatorResultViewModel
{
    public double Number1 { get; set; }
    public double Number2 { get; set; }
    public string Operation { get; set; } = string.Empty;
    public double Result { get; set; }
    public string Expression { get; set; } = string.Empty;
}
```

**役割:**
- 計算結果をViewに渡すための構造
- 表示に必要な情報をまとめる

### 2. Controller 層

**CalculatorController.cs**
```csharp
public class CalculatorController : Controller
{
    // GET: フォーム表示
    public IActionResult Index()
    {
        return View();
    }
    
    // POST: 計算実行
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CalculatorInputViewModel input)
    {
        // バリデーションチェック
        if (!ModelState.IsValid)
        {
            return View(input);
        }
        
        // 計算ロジック
        var result = Calculate(input.Number1!.Value, 
                               input.Number2!.Value, 
                               input.Operation!);
        
        // 結果ViewModelを作成
        var resultViewModel = new CalculatorResultViewModel
        {
            Number1 = input.Number1.Value,
            Number2 = input.Number2.Value,
            Operation = input.Operation,
            Result = result,
            Expression = BuildExpression(...)
        };
        
        return View("Result", resultViewModel);
    }
    
    private double Calculate(double num1, double num2, string operation)
    {
        return operation switch
        {
            "add" => num1 + num2,
            "subtract" => num1 - num2,
            "multiply" => num1 * num2,
            "divide" => num2 != 0 ? num1 / num2 
                        : throw new DivideByZeroException(),
            _ => throw new ArgumentException("不正な演算子")
        };
    }
}
```

**役割:**
- HTTP リクエストを受け取る
- Model Binding（自動データバインディング）
- バリデーション結果の確認
- ビジネスロジック（計算）の実行
- 適切なViewに結果を渡す

**重要ポイント:**
- `[HttpPost]`: POST リクエストのみ受け付ける
- `[ValidateAntiForgeryToken]`: CSRF攻撃対策（セキュリティ）
- `ModelState.IsValid`: Data Annotations の検証結果を確認

### 3. View 層

**Calculator/Index.cshtml** - 入力フォーム
```cshtml
@model HelloCSharp.Models.CalculatorInputViewModel

<form asp-action="Index" method="post">
    <div class="form-group">
        <label asp-for="Number1"></label>
        <input asp-for="Number1" class="form-control" type="number" />
        <span asp-validation-for="Number1" class="text-danger"></span>
    </div>
    
    <div class="form-group">
        <label asp-for="Operation"></label>
        <select asp-for="Operation" class="form-control">
            <option value="">-- 選択してください --</option>
            <option value="add">足し算 (+)</option>
            <option value="subtract">引き算 (-)</option>
            <option value="multiply">掛け算 (×)</option>
            <option value="divide">割り算 (÷)</option>
        </select>
        <span asp-validation-for="Operation" class="text-danger"></span>
    </div>
    
    <button type="submit">計算する</button>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

**Tag Helper の動き:**
- `asp-for="Number1"` → `[Display(Name)]` からラベルテキスト取得
- `asp-validation-for="Number1"` → バリデーションエラー表示エリア
- `asp-action="Index"` → フォーム送信先を /Calculator/Index に設定

**Calculator/Result.cshtml** - 結果表示
```cshtml
@model HelloCSharp.Models.CalculatorResultViewModel

<div class="result-display">
    <div class="expression">@Model.Expression</div>
    <div class="result-value">結果: @Model.Result</div>
</div>

<table>
    <tr>
        <th>1つ目の数値</th>
        <td>@Model.Number1</td>
    </tr>
    <tr>
        <th>結果</th>
        <td><strong>@Model.Result</strong></td>
    </tr>
</table>
```

**役割:**
- ViewModel のデータを HTML で表示
- `@model` ディレクティブで型を宣言
- `@Model.XXX` で IntelliSense が効く（型安全）

---

## まとめ：今回学んだこと

### 1. Data Annotations（データアノテーション）
- プロパティに属性を付けて、バリデーションや表示名を宣言的に定義
- `[Required]`, `[Display]`, `[Range]` など
- Tag Helpers と連携して自動的に HTML を生成

### 2. Model Binding（モデルバインディング）
- フォームのPOSTデータを自動的にViewModelにマッピング
- `public IActionResult Index(CalculatorInputViewModel input)` で受け取る
- 型変換も自動（文字列 → double）

### 3. Razor Section（セクション機構）
- 個別ページで `@section Scripts { ... }` を定義
- レイアウトで `@RenderSection("Scripts", required: false)` を表示
- ページ固有のスクリプトやスタイルを追加できる

### 4. Tag Helpers（タグヘルパー）
- `asp-for`, `asp-action`, `asp-validation-for` など
- HTMLライクな記述で強力な機能を提供
- IntelliSense が効いて安全

### 5. POST/Redirect/GET パターン
- POST で処理を実行
- 結果を別のView（Result.cshtml）で表示
- ブラウザの再読み込みで二重送信を防ぐ

---

## 次のステップ

このアプリをさらに発展させるアイデア：

1. **カスタムバリデーション**
   - 「Number2が0の時、割り算を選択できない」など

2. **計算履歴の保存**
   - Session や Cookie を使って過去の計算を保持

3. **単体テスト**
   - `Calculate()` メソッドのテスト
   - Controller のテスト（モック利用）

4. **エラーハンドリング改善**
   - ゼロ除算時に専用のエラーページ表示
   - try-catch での例外処理

5. **より複雑なViewModel**
   - ネストしたオブジェクト
   - リストやコレクション

6. **データベース連携**
   - EF Core で計算履歴を保存
   - CRUD操作の実装

---

このドキュメントで、Data Annotations の使い方、エラーの原因、そして電卓アプリ全体の流れが理解できたと思います。わからないことがあれば、コードを読み返したり、実際に値を変えて動作確認してみてください！
