# HelloCSharp プロジェクト構成ガイド

## 📁 フォルダ構成の全体像

```
HelloCSharp/
│
├── 📂 Areas/                    # 機能別エリア（メイン開発場所）
│   ├── Samples/                 # 学習サンプル機能
│   │   ├── Controllers/
│   │   ├── Models/
│   │   └── Views/
│   └── UserManagement/          # ユーザー管理機能（EAVモデル）
│       ├── Controllers/
│       ├── Models/
│       └── Views/
│
├── 📂 Controllers/              # ルートコントローラー（共通機能のみ）
│   └── HomeController.cs        # ホーム画面、About等
│
├── 📂 Views/                    # ルートビュー（共通機能のみ）
│   ├── Home/                    # HomeControllerのビュー
│   └── Shared/                  # 共通レイアウト（_Layout.cshtml等）
│
├── 📂 Models/                   # 共通モデル（現在は空）
│
├── 📂 Data/                     # データベース関連
│   └── AppDbContext.cs          # EF Core DbContext
│
├── 📂 Migrations/               # EF Core マイグレーション
│
├── 📂 wwwroot/                  # 静的ファイル（CSS, JS, 画像）
│
├── 📂 docs/                     # ドキュメント
│
├── Program.cs                   # エントリーポイント
├── HelloCSharp.csproj           # プロジェクト設定
├── HelloCSharp.db               # SQLiteデータベース
└── README.md                    # プロジェクト概要
```

---

## 🤔 なぜルートとAreasの両方にMVCがあるのか？

### 結論：**正しい設計です**

| 場所 | 役割 | 例 |
|------|------|-----|
| **ルート（Controllers/, Views/）** | 共通機能、エントリーポイント | Home, About, Error, Login |
| **Areas/** | 機能別モジュール | Samples, UserManagement, Admin等 |

### 実務での使い分け

```
/ (ルート)
├── Home        → トップページ、ランディング
├── Account     → ログイン/ログアウト（認証）
├── Error       → エラーページ
└── About       → 会社概要等

/Areas/Admin/   → 管理者専用機能
/Areas/User/    → 一般ユーザー機能
/Areas/API/     → Web API
```

### このプロジェクトでの設計

```
/ (ルート)
└── Home           → トップページ、各機能へのナビゲーション

/Areas/Samples/    → MVC学習サンプル（Calculator, BMI）
/Areas/UserManagement/ → EAVモデルのユーザー管理
```

**ポイント**: ルートの`HomeController`は**ハブ（中心）** の役割。各Areaへの入口。

---

## 📂 Views/Shared/ の重要性

`Views/Shared/`は**全てのArea共通**のレイアウトを置く場所：

```
Views/
└── Shared/
    ├── _Layout.cshtml           # 共通レイアウト（ヘッダー、フッター）
    ├── _ValidationScriptsPartial.cshtml  # バリデーション用JS
    └── Error.cshtml             # エラーページ
```

Areas内のViewは`_ViewStart.cshtml`で`Layout = "_Layout"`を指定すると、
自動的に`Views/Shared/_Layout.cshtml`を参照します。

---

## 🎯 このプロジェクトの設計意図

### 学習段階別の構成

| フェーズ | 場所 | 内容 |
|---------|------|------|
| **Step 1** | Areas/Samples/ | MVC基礎（Calculator, BMI） |
| **Step 2** | Areas/UserManagement/ | DB連携、EAVモデル、CRUD |
| **Step 3** | （将来）Areas/API/ | Web API、TypeScript連携 |

### なぜAreasを使うか？

1. **スケーラビリティ**: 機能追加時にフォルダを追加するだけ
2. **独立性**: 各Areaは独自のModels/Views/Controllersを持つ
3. **チーム開発**: 機能ごとに担当者を分けやすい
4. **名前衝突防止**: `UserController`が複数Areaにあっても問題なし

---

## 🔄 もし「全部Areasに統一したい」場合

HomeControllerもAreaに移動することは可能ですが、**推奨しません**：

```
# ❌ 非推奨パターン
Areas/
├── Common/          # 共通機能をAreaに
│   └── Controllers/HomeController.cs
├── Samples/
└── UserManagement/
```

**理由**:
- ルートURL `/` へのマッピングが複雑になる
- `Views/Shared/_Layout.cshtml`の参照が複雑になる
- ASP.NET Coreの規約から外れる

---

## ✅ 現在の構成が適切な理由

```
✅ ルート: 共通機能（Home, Error, 将来のLogin）
✅ Areas: 機能別モジュール（Samples, UserManagement）
✅ Views/Shared: 共通レイアウト
✅ Data: DBアクセス層（DIで全Areaから利用可能）
```

**これはASP.NET Core MVCの標準的なベストプラクティスです。**

---

## 📝 補足: URLとフォルダの対応

| URL | Controller | 場所 |
|-----|------------|------|
| `/` | HomeController | Controllers/ |
| `/Home/About` | HomeController | Controllers/ |
| `/Samples/Calculator` | CalculatorController | Areas/Samples/Controllers/ |
| `/Samples/Bmi` | BmiController | Areas/Samples/Controllers/ |
| `/UserManagement/Attribute` | AttributeController | Areas/UserManagement/Controllers/ |
| `/UserManagement/User` | UserController | Areas/UserManagement/Controllers/ |
