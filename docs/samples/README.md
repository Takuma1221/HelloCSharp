# Samples - MVC学習サンプル集

このフォルダには、ASP.NET Core MVCの基礎を学ぶためのサンプルアプリケーションと関連ドキュメントが含まれています。

## 📚 学習ドキュメント

### 1. mvc-for-beginners.md
- **対象**: MVC初学者
- **内容**: MVCパターンの基本概念、Model/View/Controllerの役割、データフロー図解
- **推奨順序**: 最初に読むべきドキュメント

### 2. mvc_basics.md
- **対象**: コードリーディング学習者
- **内容**: `Program.cs`の詳細解説、ミドルウェアパイプライン、ルーティング設定
- **推奨順序**: 2番目

### 3. calculator-app-explanation.md
- **対象**: 実践的なMVCアプリケーション開発者
- **内容**: Data Annotations、ModelState検証、Razorセクション、ViewModelパターン
- **推奨順序**: 3番目（実装後の復習に最適）

## 🎯 サンプルアプリケーション

Areas/Samplesに以下のアプリケーションが実装されています：

### 1. 計算機アプリ (`/Samples/Calculator`)
- **機能**: 四則演算（加算、減算、乗算、除算）
- **学習ポイント**:
  - Model Binding（フォームデータの自動マッピング）
  - Data Annotations（[Required], [Display]）
  - サーバーサイド検証（ModelState.IsValid）
  - クライアントサイド検証（jQuery Validation）
  - POST/Redirect/GET パターン
  - ViewModelの使い分け（Input/Result）

**主要ファイル**:
- `Controllers/CalculatorController.cs`
- `Models/CalculatorInputViewModel.cs`
- `Models/CalculatorResultViewModel.cs`
- `Views/Calculator/Index.cshtml`
- `Views/Calculator/Result.cshtml`

### 2. BMI計算アプリ (`/Samples/Bmi`)
- **機能**: 体重・身長からBMI計算、カテゴリ判定
- **学習ポイント**:
  - [Range]バリデーション（数値範囲チェック）
  - Switch式によるパターンマッチング（`case < 18.5:`）
  - Math.Round()を使った計算精度制御
  - 独自ビジネスロジック実装

**主要ファイル**:
- `Controllers/BmiController.cs`
- `Models/BmiInputViewModel.cs`
- `Models/BmiResultViewModel.cs`
- `Views/Bmi/Index.cshtml`
- `Views/Bmi/Result.cshtml`

## 🚀 動作確認

```bash
# プロジェクトルートで実行
dotnet build
dotnet run

# ブラウザでアクセス
# 計算機: http://localhost:5000/Samples/Calculator
# BMI: http://localhost:5000/Samples/Bmi
```

## 📖 学習の進め方

1. **ドキュメント読解** (30分)
   - `mvc-for-beginners.md` → MVCの全体像把握
   - `mvc_basics.md` → Program.cs理解

2. **コードリーディング** (1時間)
   - `CalculatorController.cs` → アクションメソッドの流れ
   - `CalculatorInputViewModel.cs` → Data Annotations
   - `Index.cshtml` → Tag Helpers、検証表示

3. **実装練習** (1時間)
   - BMIアプリを参考に、独自の計算アプリを実装
   - 例: 割引計算機、消費税計算機、単位変換など

4. **発展学習** (次のステップ)
   - データベース連携（Entity Framework Core + SQLite）
   - **EAVモデル**でユーザー管理システムを学ぶ（`docs/user-management`参照）
   - 動的スキーマ設計の理解

## 💡 重要な概念

### Model Binding
フォームの入力値が自動的にViewModelのプロパティにマッピングされる仕組み
```csharp
public IActionResult Index(CalculatorInputViewModel input) // 自動バインディング
```

### Data Annotations
属性によるデータ検証ルール定義
```csharp
[Required(ErrorMessage = "入力してください")]
[Range(0.1, 300.0, ErrorMessage = "範囲エラー")]
public double? Value { get; set; }
```

### ViewModelパターン
- **Input ViewModel**: ユーザー入力用（検証ルール付き）
- **Result ViewModel**: 結果表示用（計算結果、メッセージ）
- 目的別に分離することで責務が明確化

### CSRF対策
```csharp
[ValidateAntiForgeryToken] // POSTアクションに必須
```

## 🔧 トラブルシューティング

### エラー: "sections 'Scripts' defined but not rendered"
→ `_Layout.cshtml`に`@RenderSection("Scripts", required: false)`追加

### エラー: nullable参照型警告
→ `double?`（nullable value type）を使用し、[Required]と組み合わせる

### ビルドエラー: namespace not found
→ `_ViewImports.cshtml`で`@using HelloCSharp.Areas.Samples.Models`追加

## 次のステップ

Samplesでの学習が完了したら、`docs/user-management`でEAVモデルとデータベース連携を学びましょう！
