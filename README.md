# EAVユーザー管理システム ドキュメント

## 📚 ドキュメント構成

```
docs/
├── 📖 README.md                    # このファイル（目次）
│
├── 📂 architecture/                # 設計・構成ガイド
│   └── folder-structure.md         # フォルダ構成の解説
│
├── 📂 user-management/             # EAVモデル・ユーザー管理
│   ├── requirements.md             # 要件定義
│   ├── er-diagram.md               # ER図・DB設計
│   ├── implementation-steps.md     # 実装手順書
│   └── frontend-guide.md           # フロントエンド（React）ガイド
│
└── 📂 notes/                       # 学習メモ（自分用）
    └── (ここに自分のメモを追加)
```

---

## 🎯 このプロジェクトについて

**EAV（Entity-Attribute-Value）モデル**を使ったユーザー管理システムの学習プロジェクトです。

### 技術スタック

| 領域 | 技術 |
|------|------|
| バックエンド | ASP.NET Core MVC (.NET 9.0), Web API |
| ORM | Entity Framework Core / 生SQL (Microsoft.Data.Sqlite) |
| DB | SQLite |
| フロントエンド | React 19, TypeScript |
| 状態管理 | React Query, Jotai |
| テーブルUI | TanStack Table |
| UI | Bootstrap 5 |
| ビルド | esbuild |
| ログ | Serilog |
| バリデーション | FluentValidation |
| アーキテクチャ | CQRS (MediatR) |

---

## 📊 ドキュメント一覧

| ドキュメント | 内容 | 難易度 |
|-------------|------|--------|
| **設計** |
| `architecture/folder-structure.md` | Areas、フォルダ構成 | ⭐⭐ |
| **DB連携** |
| `user-management/requirements.md` | EAVモデル要件 | ⭐⭐⭐ |
| `user-management/er-diagram.md` | テーブル設計、リレーション | ⭐⭐⭐⭐ |
| `user-management/implementation-steps.md` | CRUD実装手順 | ⭐⭐⭐⭐⭐ |
| `user-management/frontend-guide.md` | React + Web API | ⭐⭐⭐⭐ |
| **段階的強化** |
| `phase1-implementation.md` | Serilog + FluentValidation | ⭐⭐⭐ |
| `phase2-implementation.md` | CQRS + MediatR + Pipeline Behaviors | ⭐⭐⭐⭐ |
| `phase3-implementation.md` | React Query + Jotai + TanStack Table | ⭐⭐⭐⭐ |

---

## ✅ 実装状況

### Phase 1: ログ & バリデーション基盤（完了）
- [x] Serilog導入（構造化ログ、ファイル出力）
- [x] FluentValidation導入（モデルバリデーション）
- [x] 5つのバリデータ実装
- [x] ModelState自動統合

### Phase 2: CQRS + MediatR（完了）
- [x] MediatR導入
- [x] Command/Query分離
- [x] Handlers実装（5種類）
- [x] Pipeline Behaviors（Logging, Performance, Validation, Exception）
- [x] AttributeSqlController → MediatR対応

### Phase 3: フロントエンド強化（完了）
- [x] React Query導入（サーバーステート管理）
- [x] Jotai導入（クライアントステート管理）
- [x] TanStack Table導入（ソート、フィルタ、ページング）
- [x] AttributePageV2実装
- [x] React Query DevTools統合

### 未実装
- [ ] ユーザー管理CRUDをPhase 3対応
- [ ] Optimistic Updates
- [ ] React Hook Form + Zod
- [ ] Storybook導入

---

## � クイックスタート

### 開発サーバー起動

```bash
# バックエンド起動
dotnet run
# → http://localhost:5000

# フロントエンド（別ターミナル）
npm run watch:attribute  # 監視ビルド
```

### アクセス先

| ページ | URL | 説明 |
|--------|-----|------|
| ホーム | http://localhost:5000 | トップページ |
| **属性管理（Phase 3強化版）** | **http://localhost:5000/Attribute** | **最新版** |

### ビルドコマンド

```bash
# フロントエンドビルド
npm run build:attribute          # 本番ビルド
npm run build:attribute:dev      # 開発ビルド（sourcemap付き）
npm run watch:attribute          # 監視モード
```

---

## �📝 学習メモの作成方法

`docs/notes/`フォルダに自分の学習メモを追加できます：

```markdown
# 学習メモ: [トピック名]

日付: 2025/11/30

## 学んだこと
-

## わからなかったこと
-

## 次にやること
-
```

---

## 🔗 クイックリンク

### プロジェクト内
- **段階的強化**
  - [Phase 1: Serilog + FluentValidation](./docs/phase1-implementation.md)
  - [Phase 2: CQRS + MediatR + Pipeline Behaviors](./docs/phase2-implementation.md)
  - [Phase 3: React Query + Jotai + TanStack Table](./docs/phase3-implementation.md)
- **設計・実装**
  - [フォルダ構成](./docs/architecture/folder-structure.md)
  - [EAV要件定義](./docs/user-management/requirements.md)
  - [実装手順](./docs/user-management/implementation-steps.md)

### 外部リソース
- [ASP.NET Core 公式ドキュメント](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [React 公式](https://react.dev/)
- [React Query (TanStack Query)](https://tanstack.com/query/latest)
- [Jotai](https://jotai.org/)
- [TanStack Table](https://tanstack.com/table/latest)

---

## � 進化の歴史

| フェーズ | 実装内容 | 主な効果 |
|---------|---------|---------|
| **初期** | ASP.NET Core MVC + React + SQLite | 基本CRUD完成 |
| **Phase 1** | Serilog + FluentValidation | ログ基盤、バリデーション強化 |
| **Phase 2** | CQRS + MediatR + Pipeline Behaviors | アーキテクチャ整理、Handler60%削減 |
| **Phase 3** | React Query + Jotai + TanStack Table | キャッシュ、ソート/フィルタ、DevTools |

詳細は各フェーズのドキュメントを参照してください。

---

## �📂 アーカイブについて

学習初期に使用していたドキュメントは `../HelloCSharp_archive/docs/` に移動しています：
- `samples/` - 電卓・BMIサンプルの解説
- `learning/` - DI、ORM基礎
- `themes.md` - VS Codeテーマ設定
- `setup.md` - 環境セットアップ
