# Copilot Instructions - HelloCSharp

## プロジェクト概要

EAV（Entity-Attribute-Value）モデルを使ったユーザー管理システム。  
**ASP.NET Core MVC + React SPA** のハイブリッド構成。

## アーキテクチャ

### データフロー

```
cshtml (HTML shell) → React (tsx) → fetch API → ApiController (C#) → SQLite
```

- **View層**: cshtml は React マウント用の `<div id="react-root">` のみ
- **フロントエンド**: React 19 + TypeScript、esbuild でバンドル
- **API**: `[ApiController]` で JSON を返す（現在は生SQL版を使用）
- **DB**: SQLite (`HelloCSharp.db`)、生SQL (`Microsoft.Data.Sqlite`)

### ファイル対応規則

| cshtml | tsx | 出力js |
|--------|-----|--------|
| `Views/Home/Index.cshtml` | `Scripts/react/pages/HomePage.tsx` | `home-page.js` |
| `Areas/.../Attribute/Index.cshtml` | `Scripts/react/pages/AttributePage.tsx` | `attribute-page.js` |

新規ページ追加時: `pages/{Name}Page.tsx` → `{name}-page.js`

## 開発コマンド

```bash
# .NET ビルド・実行
dotnet build
dotnet run                    # http://localhost:5000

# React ビルド（両方必要）
npm run build:dev             # 開発ビルド（sourcemap付き）
npm run watch:attribute       # 属性管理のみ監視

# DB マイグレーション（EF Core参考用）
dotnet ef migrations add <Name>
dotnet ef database update
```

## ディレクトリ構成

```
Scripts/react/
├── pages/          # エントリーポイント（1ページ=1ファイル）
├── components/     # 再利用可能な UI コンポーネント
└── shared/         # api.ts, types.ts（共通モジュール）

Areas/UserManagement/
├── Controllers/Api/    # Web API（AttributeSqlController.cs が現在使用中）
├── Models/             # Entity（AttributeDefinition, User, UserAttributeValue）
└── Views/              # React マウント用 cshtml
```

## コーディング規約

### C# (API Controller)

- 生SQL版: `Microsoft.Data.Sqlite` + パラメータ化クエリ（SQLインジェクション対策）
- 接続文字列: `Data Source=HelloCSharp.db`
- レスポンス: `Ok()`, `NotFound()`, `BadRequest()` を使用

```csharp
// 例: パラメータ化クエリ
command.Parameters.AddWithValue("@id", id);
```

### React (TypeScript)

- 型定義: `Scripts/react/shared/types.ts` に集約
- API呼び出し: `Scripts/react/shared/api.ts` の `attributeApi` を使用
- 状態管理: `useState`, `useCallback`, `useEffect`（Redux不使用）

```tsx
// ページコンポーネントのパターン
const XxxPage: React.FC = () => {
    const [data, setData] = useState<T[]>([]);
    // ... CRUD handlers
};
// DOMマウント
const container = document.getElementById('react-root');
```

### cshtml

- React用: `<div id="react-root">` + `@section Scripts { <script src="..."> }`
- コメントで対応tsx明記: `@* 対応: Scripts/react/pages/XxxPage.tsx *@`

## EAVモデル

3テーブル構成:
- `Users`: ユーザー基本情報
- `Attributes`: 属性定義（データ型、必須フラグ等）
- `UserAttributeValues`: ユーザーと属性の値（多対多）

## 注意事項

- EF Core版 (`AttributeApiController.cs`) は参考用、現在は生SQL版を使用
- `HelloCSharp_archive/` にアーカイブ済みの学習初期ファイルあり
- Bootstrap 5 のクラスを使用（独自CSSは `wwwroot/css/site.css`）
