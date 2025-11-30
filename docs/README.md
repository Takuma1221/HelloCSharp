# HelloCSharp ドキュメント目次

## 📚 ドキュメント構成

```
docs/
├── 📖 README.md                    # このファイル（目次）
│
├── 📂 architecture/                # 設計・構成ガイド
│   └── folder-structure.md         # フォルダ構成の解説
│
├── 📂 learning/                    # 基礎概念の学習
│   ├── README.md                   # 学習ガイド目次
│   ├── dependency-injection.md     # DI（依存性注入）
│   └── sql-to-orm.md               # 生SQL → Dapper → EF Core
│
├── 📂 samples/                     # MVC基礎サンプル解説
│   ├── README.md                   # サンプルアプリ概要
│   ├── mvc-for-beginners.md        # MVC初心者ガイド
│   ├── mvc_basics.md               # MVC詳細解説
│   └── calculator-app-explanation.md # 電卓アプリ解説
│
├── 📂 user-management/             # EAVモデル・ユーザー管理
│   ├── requirements.md             # 要件定義
│   ├── er-diagram.md               # ER図・DB設計
│   ├── implementation-steps.md     # 実装手順書
│   └── frontend-guide.md           # フロントエンド連携
│
├── 📂 notes/                       # 学習メモ（自分用）
│   └── (ここに自分のメモを追加)
│
├── setup.md                        # 環境セットアップ
└── themes.md                       # VS Code テーマ設定
```

---

## 🎯 学習ロードマップ

### Phase 1: 環境構築（10分）
- [ ] `setup.md` - 開発環境のセットアップ
- [ ] `themes.md` - VS Codeの設定

### Phase 2: MVC基礎（2時間）
- [ ] `samples/mvc-for-beginners.md` - MVCパターンの概念
- [ ] `samples/mvc_basics.md` - ASP.NET Core MVCの詳細
- [ ] `samples/calculator-app-explanation.md` - 実装の解説
- [ ] `samples/README.md` - サンプルアプリを動かす

### Phase 3: 基礎概念（1時間）
- [ ] `learning/dependency-injection.md` - DIとは何か
- [ ] `learning/sql-to-orm.md` - データベースアクセス方法

### Phase 4: 設計理解（30分）
- [ ] `architecture/folder-structure.md` - プロジェクト構成

### Phase 5: DB連携実装（3-4時間）
- [ ] `user-management/requirements.md` - EAVモデルとは
- [ ] `user-management/er-diagram.md` - データベース設計
- [ ] `user-management/implementation-steps.md` - 実装手順

### Phase 6: 応用（将来）
- [ ] `user-management/frontend-guide.md` - TypeScript連携

---

## 📊 ドキュメント一覧

| ドキュメント | 内容 | 難易度 | 時間 |
|-------------|------|--------|------|
| **環境構築** |
| `setup.md` | .NET SDK、VS Code設定 | ⭐ | 10分 |
| `themes.md` | エディタテーマ | ⭐ | 5分 |
| **MVC基礎** |
| `samples/mvc-for-beginners.md` | MVCパターン入門 | ⭐⭐ | 30分 |
| `samples/mvc_basics.md` | Program.cs、ルーティング | ⭐⭐⭐ | 45分 |
| `samples/calculator-app-explanation.md` | バリデーション、ViewModel | ⭐⭐⭐ | 1時間 |
| **基礎概念** |
| `learning/dependency-injection.md` | DI、サービスライフタイム | ⭐⭐⭐ | 30分 |
| `learning/sql-to-orm.md` | ADO.NET → Dapper → EF Core | ⭐⭐⭐⭐ | 1時間 |
| **設計** |
| `architecture/folder-structure.md` | Areas、フォルダ構成 | ⭐⭐ | 15分 |
| **DB連携** |
| `user-management/requirements.md` | EAVモデル要件 | ⭐⭐⭐ | 30分 |
| `user-management/er-diagram.md` | テーブル設計、リレーション | ⭐⭐⭐⭐ | 45分 |
| `user-management/implementation-steps.md` | CRUD実装手順 | ⭐⭐⭐⭐⭐ | 3時間 |
| `user-management/frontend-guide.md` | TypeScript + Web API | ⭐⭐⭐⭐ | 2時間 |

---

## 📝 学習メモの作成方法

`docs/notes/`フォルダに自分の学習メモを追加できます：

```bash
# 例: 学習メモを作成
touch docs/notes/2025-11-30-di-learning.md
```

### メモのテンプレート

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

### よく参照するドキュメント
- [フォルダ構成](./architecture/folder-structure.md) - プロジェクト構成の理解
- [DI解説](./learning/dependency-injection.md) - DIパターン
- [実装手順](./user-management/implementation-steps.md) - Step by Step実装

### 外部リソース
- [ASP.NET Core 公式ドキュメント](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [Razor 構文](https://learn.microsoft.com/aspnet/core/mvc/views/razor)

---

## 📅 更新履歴

| 日付 | 内容 |
|------|------|
| 2025/11/30 | ドキュメント再構成、architecture/追加、notes/追加 |
| 2025/11/08 | Areas構造対応、user-management/追加 |
| 初版 | MVC基礎ドキュメント作成 |
