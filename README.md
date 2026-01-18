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
| バックエンド | ASP.NET Core MVC, Web API |
| ORM | Entity Framework Core |
| DB | SQLite |
| フロントエンド | React 19, TypeScript |
| UI | Bootstrap 5 |
| ビルド | esbuild |

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

---

## ✅ 実装状況

### 完了
- [x] プロジェクト構築（ASP.NET Core MVC）
- [x] Entity Framework Core + SQLite セットアップ
- [x] EAVモデルのEntity作成（User, Attribute, UserAttributeValue）
- [x] AppDbContext + マイグレーション
- [x] Web API（AttributeApiController）
- [x] React版 属性管理CRUD

### 未実装
- [ ] ユーザー管理CRUD（React版）
- [ ] 動的フォーム生成（属性に応じた入力フォーム）
- [ ] ユーザー詳細画面

---

## 📝 学習メモの作成方法

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
- [フォルダ構成](./architecture/folder-structure.md)
- [EAV要件定義](./user-management/requirements.md)
- [実装手順](./user-management/implementation-steps.md)

### 外部リソース
- [ASP.NET Core 公式ドキュメント](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [React 公式](https://react.dev/)

---

## 📂 アーカイブについて

学習初期に使用していたドキュメントは `../HelloCSharp_archive/docs/` に移動しています：
- `samples/` - 電卓・BMIサンプルの解説
- `learning/` - DI、ORM基礎
- `themes.md` - VS Codeテーマ設定
- `setup.md` - 環境セットアップ
