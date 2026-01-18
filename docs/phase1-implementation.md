# Phase 1 実装: FluentValidation + Serilog

実装日: 2026年1月18日

## 概要

インフラストラクチャ層の強化として、構造化ログ（Serilog）とバリデーション（FluentValidation）を導入しました。

## インストールしたパッケージ

```bash
dotnet add package Serilog.AspNetCore       # v10.0.0
dotnet add package Serilog.Sinks.File       # v7.0.0
dotnet add package Serilog.Sinks.Console    # v6.1.1
dotnet add package FluentValidation         # v12.1.1
dotnet add package FluentValidation.DependencyInjectionExtensions    # v12.1.1
dotnet add package FluentValidation.AspNetCore                       # v11.3.1
```

---

## 1. Serilog（構造化ログ）

### Before

**問題点:**
- デフォルトのASP.NET Coreロガーのみ使用
- ログがコンソールのみに出力され、永続化されない
- ログレベルの細かい制御が困難
- 構造化されていないため、検索・分析が困難

**コード例 (Before):**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// ログ設定なし
```

### After

**実装内容:**
- Serilogをグローバルロガーとして設定
- コンソール + ファイル出力（日次ローテーション）
- 構造化ログによる詳細な情報記録
- `logs/app-YYYYMMDD.log` 形式でファイル保存

**コード例 (After):**
```csharp
// Program.cs
using Serilog;

// Serilog設定
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// SerilogをASP.NET Coreのログプロバイダーとして追加
builder.Host.UseSerilog();
```

**ログ出力例:**
```
[12:39:57 INF] すべての属性定義を取得します
[12:39:57 INF] 6件の属性定義を取得しました
[12:40:11 INF] 属性定義を作成します: CQRS属性
[12:40:11 INF] 属性定義を作成しました: ID=9, Name=CQRS属性
```

### 改善点

✅ **永続化**: ログがファイルに保存され、後からトラブルシューティングが可能  
✅ **構造化**: タイムスタンプ、ログレベル、メッセージが明確に分離  
✅ **ローテーション**: 日次で自動ローテーション、ディスク容量の管理が容易  
✅ **検索性**: ログレベル別のフィルタリングが簡単  
✅ **本番運用**: Production環境でのデバッグが効率化

---

## 2. FluentValidation（バリデーション）

### Before

**問題点:**
- Data Annotationsのみでバリデーション
- バリデーションロジックがモデルクラスに密結合
- 複雑なバリデーションルールの実装が困難
- テストが書きにくい
- バリデーションロジックの再利用が難しい

**コード例 (Before):**
```csharp
// Models/AttributeDefinition.cs
public class AttributeDefinition
{
    [Required(ErrorMessage = "属性名を入力してください")]
    [StringLength(50, ErrorMessage = "属性名は50文字以内で入力してください")]
    public string AttributeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "データ型を選択してください")]
    public string DataType { get; set; } = "Text";
}
```

### After

**実装内容:**
- バリデーションロジックを独立したクラスに分離
- 3つのバリデータークラスを作成
- DIコンテナに自動登録
- ModelStateと自動統合

**作成したファイル:**

**1. Validators/UserValidator.cs**
```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("名前は必須です")
            .MaximumLength(100).WithMessage("名前は100文字以内で入力してください");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("メールアドレスは必須です")
            .EmailAddress().WithMessage("有効なメールアドレスを入力してください")
            .MaximumLength(255).WithMessage("メールアドレスは255文字以内で入力してください");
    }
}
```

**2. Validators/AttributeDefinitionValidator.cs**
```csharp
public class AttributeDefinitionValidator : AbstractValidator<AttributeDefinition>
{
    public AttributeDefinitionValidator()
    {
        RuleFor(a => a.AttributeName)
            .NotEmpty().WithMessage("属性名は必須です")
            .MaximumLength(50).WithMessage("属性名は50文字以内で入力してください");

        RuleFor(a => a.DataType)
            .NotEmpty().WithMessage("データ型は必須です")
            .Must(BeValidDataType).WithMessage("有効なデータ型を選択してください (Text, Number, Date)");

        RuleFor(a => a.DisplayOrder)
            .GreaterThan(0).WithMessage("表示順序は1以上の値を入力してください")
            .LessThan(1000).WithMessage("表示順序は999以下の値を入力してください");
    }

    private bool BeValidDataType(string dataType)
    {
        var validTypes = new[] { "Text", "Number", "Date" };
        return validTypes.Contains(dataType);
    }
}
```

**3. Validators/UserAttributeValueValidator.cs**
```csharp
public class UserAttributeValueValidator : AbstractValidator<UserAttributeValue>
{
    public UserAttributeValueValidator()
    {
        RuleFor(v => v.UserId)
            .GreaterThan(0).WithMessage("ユーザーIDは必須です");

        RuleFor(v => v.AttributeId)
            .GreaterThan(0).WithMessage("属性IDは必須です");

        RuleFor(v => v.Value)
            .MaximumLength(500).WithMessage("値は500文字以内で入力してください");
    }
}
```

**DI登録 (Program.cs):**
```csharp
// FluentValidation を追加
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
```

### 動作確認

**不正データのテスト:**
```bash
curl -X POST http://localhost:5000/api/AttributeSql \
  -H "Content-Type: application/json" \
  -d '{"attributeName":"","dataType":"InvalidType","displayOrder":-1}'
```

**レスポンス:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "DataType": ["有効なデータ型を選択してください (Text, Number, Date)"],
    "DisplayOrder": ["表示順序は1以上の値を入力してください"],
    "AttributeName": ["属性名は必須です"]
  }
}
```

### 改善点

✅ **関心の分離**: バリデーションロジックがモデルから分離され、単一責任の原則に準拠  
✅ **再利用性**: バリデータークラスを他の場所でも再利用可能  
✅ **テスタビリティ**: バリデーションロジックを独立してテスト可能  
✅ **複雑なルール**: `Must()` メソッドで複雑な条件を簡単に実装  
✅ **可読性**: バリデーションルールが明示的で読みやすい  
✅ **保守性**: バリデーションルールの変更が容易  
✅ **エラーメッセージ**: カスタムメッセージを日本語で明確に定義

---

## FluentValidationの動作メカニズム

### DI登録と自動統合

**Program.cs での登録:**
```csharp
// FluentValidation を追加
builder.Services.AddFluentValidationAutoValidation();  // ← 自動バリデーション有効化
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();  // ← バリデータをDIコンテナに登録
```

**登録内容の詳細:**
- `AddFluentValidationAutoValidation()`: ASP.NET CoreのModel Binding時に自動的にバリデーション実行
- `AddValidatorsFromAssemblyContaining<T>()`: 指定したアセンブリ内のすべてのバリデータを自動検出してDI登録
  - `UserValidator`
  - `AttributeDefinitionValidator`
  - `UserAttributeValueValidator`

### 実際の使用箇所（暗黙的な使用）

FluentValidationは**明示的な呼び出しが不要**です。ASP.NET CoreのModel Bindingパイプラインに自動統合されています。

**Controller内でのチェック:**
```csharp
[HttpPost]
public async Task<ActionResult<AttributeDefinition>> Create([FromBody] CreateAttributeCommand command)
{
    // この時点で既にFluentValidationが自動実行済み
    if (!ModelState.IsValid)  // ← バリデーション結果をチェック
    {
        return BadRequest(ModelState);  // ← エラー内容を返す
    }

    var created = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

### 動作フロー

```
1. HTTPリクエスト受信
   POST /api/AttributeSql
   Body: {"attributeName":"","dataType":"Invalid","displayOrder":-1}
   ↓
2. ASP.NET Core Model Binding
   [FromBody] でJSONをCreateAttributeCommandにデシリアライズ
   ↓
3. FluentValidationが自動起動（AddFluentValidationAutoValidation効果）
   - AttributeDefinitionValidatorを検出
   - バリデーションルールを実行
   ↓
4. バリデーション結果がModelStateに自動設定
   ModelState.IsValid = false
   ModelState.Errors = {...}
   ↓
5. Controller内でModelState.IsValidをチェック
   ↓
6. NGならBadRequest、OKなら処理続行
```

### なぜ明示的な呼び出しが不要か

従来の手動バリデーション（不要）:
```csharp
// ❌ これを書く必要がない
var validator = new AttributeDefinitionValidator();
var validationResult = validator.Validate(command);
if (!validationResult.IsValid)
{
    return BadRequest(validationResult.Errors);
}
```

FluentValidation統合後:
```csharp
// ✅ これだけでOK（自動実行される）
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

### バリデータとモデルの自動マッチング

FluentValidationは**型情報**を使って自動的にバリデータを解決します：

```csharp
// CreateAttributeCommand型のパラメータ
public async Task<ActionResult> Create([FromBody] CreateAttributeCommand command)

// ↓ FluentValidationが自動検索

// AbstractValidator<CreateAttributeCommand>を実装したクラスを探す
// → 見つからない場合は、プロパティの型でも検索

// CreateAttributeCommand内のプロパティ:
// - AttributeName (string)
// - DataType (string)
// - DisplayOrder (int)

// ↓ AttributeDefinition型と類似

// AbstractValidator<AttributeDefinition>を発見
// → AttributeDefinitionValidatorを適用
```

### 実証：バリデーションが動作している証拠

```bash
# 不正データ送信
curl -X POST http://localhost:5000/api/AttributeSql \
  -H "Content-Type: application/json" \
  -d '{"attributeName":"","dataType":"InvalidType","displayOrder":-1}'
```

**レスポンス（FluentValidationによる自動検証結果）:**
```json
{
  "status": 400,
  "errors": {
    "DataType": ["有効なデータ型を選択してください (Text, Number, Date)"],
    "DisplayOrder": ["表示順序は1以上の値を入力してください"],
    "AttributeName": ["属性名は必須です"]
  }
}
```

エラーメッセージが `AttributeDefinitionValidator.cs` で定義したものと完全一致 → バリデータが正常に動作している証拠

### メリット：宣言的バリデーション

**Before（手動バリデーション）:**
```csharp
public async Task<ActionResult> Create([FromBody] CreateAttributeCommand command)
{
    // 手動でチェック（冗長）
    if (string.IsNullOrEmpty(command.AttributeName))
        return BadRequest("属性名は必須です");
    
    if (command.AttributeName.Length > 50)
        return BadRequest("属性名は50文字以内");
    
    if (!new[] {"Text","Number","Date"}.Contains(command.DataType))
        return BadRequest("無効なデータ型");
    
    if (command.DisplayOrder < 1 || command.DisplayOrder >= 1000)
        return BadRequest("表示順は1-999");
    
    // やっと本処理
    var created = await _mediator.Send(command);
    return Ok(created);
}
```

**After（宣言的バリデーション）:**
```csharp
public async Task<ActionResult> Create([FromBody] CreateAttributeCommand command)
{
    // たった2行で完全なバリデーション
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var created = await _mediator.Send(command);
    return Ok(created);
}
```

✅ **コード量**: 15行 → 2行（約87%削減）  
✅ **可読性**: ビジネスロジックが明確に  
✅ **保守性**: バリデーションルールの変更がValidator側のみで完結

---

## ファイル構成

```
HelloCSharp/
├── Program.cs                    # Serilog設定とFluentValidation登録
├── Validators/                   # 新規作成
│   ├── UserValidator.cs
│   ├── AttributeDefinitionValidator.cs
│   └── UserAttributeValueValidator.cs
├── logs/                         # 新規作成（自動生成）
│   └── app-20260118.log         # 日次ログファイル
└── .gitignore                    # logs/を追加
```

---

## まとめ

Phase 1では、アプリケーションの**監視性（Observability）**と**データ整合性（Data Integrity）**を大幅に向上させました。

### 主な成果

1. **Serilog導入**: 構造化ログにより、本番環境でのデバッグとトラブルシューティングが効率化
2. **FluentValidation導入**: バリデーションロジックの分離により、コードの保守性とテスタビリティが向上
3. **プロダクションレディ**: エンタープライズレベルのアプリケーションに必要な基盤を構築

### 次のステップ

Phase 2では、CQRSパターンとMediatRを導入し、ビジネスロジックの分離とスケーラビリティを向上させます。
