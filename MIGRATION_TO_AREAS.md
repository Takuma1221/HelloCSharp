# プロジェクト再編成完了 - Areas構造への移行

## ✅ 実施内容

### 1. Areas構造の導入

プロジェクトを機能別にAreasで分離し、スケーラブルな構成に変更しました。

```
HelloCSharp/
├── Areas/
│   ├── Samples/          # 基礎学習用サンプル（Calculator, BMI）
│   └── UserManagement/   # ユーザー管理システム（EAVモデル・実装予定）
```

### 2. 既存ファイルの移行

**移行前**:
```
Controllers/CalculatorController.cs
Controllers/BmiController.cs
Models/Calculator/
Models/Bmi/
Views/Calculator/
Views/Bmi/
```

**移行後**:
```
Areas/Samples/Controllers/CalculatorController.cs
Areas/Samples/Controllers/BmiController.cs
Areas/Samples/Models/Calculator/
Areas/Samples/Models/Bmi/
Areas/Samples/Views/Calculator/
Areas/Samples/Views/Bmi/
```

### 3. Program.csの更新

Area対応のルーティングを追加：

```csharp
// Area routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 4. ドキュメントの整理

**docs/samples/** に基礎学習ドキュメントを集約：
- mvc-for-beginners.md
- mvc_basics.md
- calculator-app-explanation.md
- README.md（サンプル全体ガイド）

**docs/user-management/** にTodo実装ドキュメントを作成：
- requirements.md（要件定義・EAVモデル説明）
- er-diagram.md（ER図・DB設計）
- implementation-steps.md（実装手順書）

**docs/** にナビゲーションドキュメント追加：
- README.md（全体マップ、学習順序）

## 📊 変更詳細

### Controller

すべてのControllerに`[Area("エリア名")]`属性を追加：

```csharp
namespace HelloCSharp.Areas.Samples.Controllers;

[Area("Samples")]
public class CalculatorController : Controller
{
    // ...
}
```

### Models

名前空間を変更：

```csharp
// 変更前
namespace HelloCSharp.Models;

// 変更後
namespace HelloCSharp.Areas.Samples.Models;
```

### Views

各Area配下に`_ViewImports.cshtml`と`_ViewStart.cshtml`を配置：

**Areas/Samples/Views/_ViewImports.cshtml**:
```csharp
@using HelloCSharp
@using HelloCSharp.Areas.Samples.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

### ナビゲーション

`_Layout.cshtml`のURLを更新：

```html
<!-- 変更前 -->
<a href="/Calculator">電卓</a>

<!-- 変更後 -->
<a href="/Samples/Calculator">電卓</a>
```

## 🎯 新しいURL構成

| 機能 | 旧URL | 新URL |
|-----|-------|-------|
| Home | `/` | `/`（変更なし） |
| About | `/Home/About` | `/Home/About`（変更なし） |
| Calculator | `/Calculator` | `/Samples/Calculator` |
| BMI | `/Bmi` | `/Samples/Bmi` |
| ユーザー管理（予定） | - | `/UserManagement/User` |

## 📁 最終的なプロジェクト構造

```
HelloCSharp/
├── Areas/
│   ├── Samples/
│   │   ├── Controllers/
│   │   │   ├── CalculatorController.cs
│   │   │   └── BmiController.cs
│   │   ├── Models/
│   │   │   ├── Calculator/
│   │   │   │   ├── CalculatorInputViewModel.cs
│   │   │   │   └── CalculatorResultViewModel.cs
│   │   │   └── Bmi/
│   │   │       ├── BmiInputViewModel.cs
│   │   │       └── BmiResultViewModel.cs
│   │   └── Views/
│   │       ├── _ViewImports.cshtml
│   │       ├── _ViewStart.cshtml
│   │       ├── Calculator/
│   │       │   ├── Index.cshtml
│   │       │   └── Result.cshtml
│   │       └── Bmi/
│   │           ├── Index.cshtml
│   │           └── Result.cshtml
│   └── UserManagement/
│       ├── Controllers/  (準備完了)
│       ├── Models/       (準備完了)
│       └── Views/        (準備完了)
├── Controllers/
│   └── HomeController.cs
├── Data/  (これから作成)
│   └── AppDbContext.cs
├── docs/
│   ├── README.md          (新規: ドキュメント全体マップ)
│   ├── setup.md
│   ├── themes.md
│   ├── samples/
│   │   ├── README.md      (新規: サンプルガイド)
│   │   ├── mvc-for-beginners.md
│   │   ├── mvc_basics.md
│   │   └── calculator-app-explanation.md
│   └── user-management/
│       ├── requirements.md           (新規)
│       ├── er-diagram.md             (新規)
│       └── implementation-steps.md   (新規)
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── About.cshtml
│   └── Shared/
│       ├── _Layout.cshtml (更新: Area対応ナビ)
│       ├── _ViewImports.cshtml
│       ├── _ViewStart.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/
├── Program.cs (更新: Areaルーティング追加)
└── README.md  (更新: Areas構成説明)
```

## ✨ メリット

### 1. スケーラビリティ
- 機能ごとに独立したArea
- 新機能追加時に既存コードへの影響最小化

### 2. 保守性
- 関連ファイルが1箇所に集約
- 名前空間が機能と一致

### 3. 学習効率
- 基礎（Samples）と発展（TodoApp）の明確な分離
- ドキュメントも同様に整理

## 🚀 次のステップ

`docs/user-management/implementation-steps.md`に従ってユーザー管理システムを実装してください！

### Step 1: 環境準備
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet tool install --global dotnet-ef
```

### Step 2以降
`docs/user-management/implementation-steps.md`を参照

---

再編成完了日: 2025/11/08
