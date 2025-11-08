# 学習ドキュメント

このフォルダには、ASP.NET Core開発に必要な基礎概念の学習ガイドが含まれています。

## 📚 ドキュメント一覧

### 1. [Dependency Injection (依存性注入)](./dependency-injection.md)
- **対象**: DI初心者、ASP.NET Core初心者
- **内容**:
  - DIとは何か？
  - DIを使わない場合の問題点
  - ASP.NET CoreのDIコンテナ
  - サービスライフタイム（Scoped, Transient, Singleton）
  - 実装例（Repository、Controller）
- **学習時間**: 30分

### 2. [SQL → ORM 学習ガイド](./sql-to-orm.md)
- **対象**: SQL経験者、ORM初心者
- **内容**:
  - Phase 1: 生SQL（ADO.NET）で基礎を学ぶ
  - Phase 2: Dapper（軽量ORM）で効率化
  - Phase 3: Entity Framework Core（フルORM）で自動化
  - 3つの手法の比較表
  - 実装の推奨順序
- **学習時間**: 2-3時間（実装含む）

---

## 🎯 学習の進め方

### 推奨学習順序

```
1. Dependency Injection (30分)
   ↓
2. SQL → ORM - Phase 1: 生SQL (1時間)
   ↓
3. SQL → ORM - Phase 2: Dapper (1時間)
   ↓
4. SQL → ORM - Phase 3: EF Core (1時間)
   ↓
5. ユーザー管理アプリ実装 (2-3時間)
```

### Phase 1: 基礎理解（30分）
- [ ] `dependency-injection.md`を読む
- [ ] DIの3つの原則を理解
- [ ] サービスライフタイムの違いを理解

### Phase 2: SQL実装（1時間）
- [ ] `sql-to-orm.md`のPhase 1を読む
- [ ] SQLiteでテーブル作成
- [ ] `SqlUserRepository`を実装
- [ ] CRUD操作を動作確認

### Phase 3: Dapper実装（1時間）
- [ ] `sql-to-orm.md`のPhase 2を読む
- [ ] Dapperパッケージ追加
- [ ] `DapperUserRepository`を実装
- [ ] 生SQLとの違いを比較

### Phase 4: EF Core実装（1時間）
- [ ] `sql-to-orm.md`のPhase 3を読む
- [ ] EF Coreパッケージ追加
- [ ] `AppDbContext`作成
- [ ] マイグレーション実行
- [ ] `EfUserRepository`を実装

### Phase 5: アプリ完成（2-3時間）
- [ ] `docs/user-management/implementation-steps.md`を参照
- [ ] Controller、View実装
- [ ] 動的属性の追加・編集機能
- [ ] 完成！

---

## 💡 学習のコツ

### 1. 段階的に進める
一度に全てを理解しようとせず、Phase 1 → 2 → 3と順番に進めましょう。

### 2. コードを実際に書く
ドキュメントを読むだけでなく、必ず手を動かしてコードを書きましょう。

### 3. 比較して理解する
生SQL、Dapper、EF Coreの実装を並べて比較すると、それぞれの特徴がよくわかります。

### 4. エラーを恐れない
ビルドエラーやランタイムエラーが出たら、エラーメッセージをよく読んで対処しましょう。

---

## 🔗 関連ドキュメント

### ユーザー管理アプリ実装ガイド
- [要件定義](../user-management/requirements.md)
- [ER図](../user-management/er-diagram.md)
- [実装手順](../user-management/implementation-steps.md)
- [フロントエンド実装](../user-management/frontend-guide.md)

### プロジェクト全体
- [プロジェクトREADME](../../README.md)
- [Areas移行ガイド](../../MIGRATION_TO_AREAS.md)

---

## 🤔 よくある質問

### Q1: DapperとEF Core、どちらを使うべき？

**A**: 用途によります。

- **Dapper**: SQLを書きたい、パフォーマンス重視、会社のスタイルに合わせたい
- **EF Core**: 開発速度重視、マイグレーション機能が欲しい、LINQを使いたい

学習段階では両方実装して比較することをお勧めします。

### Q2: DIはいつ使う？

**A**: ASP.NET Coreでは常に使います。

- Controller、Repository、DbContext など、ほぼ全てのクラスでDIを使用
- `new`でインスタンスを作らず、コンストラクタで受け取る
- Program.csで`builder.Services.Add〜`で登録

### Q3: 生SQLは実務で使う？

**A**: 使います。

- 複雑な集計クエリ
- パフォーマンスチューニングが必要な場合
- ORMでは表現しにくいクエリ

ただし、基本的なCRUDはORMを使うことが多いです。

### Q4: Phase 1-3を全て実装する必要がある？

**A**: 学習目的なら**全て実装すること**をお勧めします。

- Phase 1: SQLの基礎理解
- Phase 2: 会社のスタイルに近い実装
- Phase 3: モダンな開発手法

実務では、プロジェクトの要件に応じてどれか1つを選択します。

---

## 📝 学習記録

学習の進捗を記録しておきましょう：

- [ ] 2025/11/08: DI学習開始
- [ ] ____/__/__: 生SQL実装完了
- [ ] ____/__/__: Dapper実装完了
- [ ] ____/__/__: EF Core実装完了
- [ ] ____/__/__: ユーザー管理アプリ完成

---

## 🚀 次のステップ

学習が完了したら、以下を試してみましょう：

1. **TypeScript + Web API**: `docs/user-management/frontend-guide.md`
2. **テスト作成**: xUnit、Moqを使った単体テスト
3. **認証・認可**: ASP.NET Core Identity
4. **デプロイ**: Azure App Service、Docker

頑張ってください！💪
