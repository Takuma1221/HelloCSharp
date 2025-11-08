# HelloCSharp ドキュメント構成

このフォルダには、HelloCSharpプロジェクトの学習用ドキュメントが格納されています。

## 📁 フォルダ構成

```
docs/
├── README.md (このファイル)
├── setup.md              # 初期セットアップ手順
├── themes.md             # VS Code テーマ設定
├── samples/              # 基礎編（MVC学習サンプル）
│   ├── README.md         # サンプルアプリ全体ガイド
│   ├── mvc-for-beginners.md
│   ├── mvc_basics.md
│   └── calculator-app-explanation.md
└── user-management/      # 発展編（EAVモデル・DB連携）
    ├── requirements.md         # 要件定義
    ├── er-diagram.md          # ER図・DB設計
    └── implementation-steps.md # 実装手順書

```

## 🎯 学習の進め方

### 1. 環境構築（所要時間: 10分）

まずは環境を整えましょう：

- **setup.md**: .NET SDK、VS Code拡張機能のインストール
- **themes.md**: 見やすい開発環境の設定

### 2. MVC基礎学習（所要時間: 2-3時間）

`docs/samples/`配下のドキュメントで基礎を固めます：

#### Step 1: 概念理解
- **samples/mvc-for-beginners.md** 
  - MVCパターンとは何か
  - Model/View/Controllerの役割
  - データフロー図解

#### Step 2: コードリーディング
- **samples/mvc_basics.md**
  - `Program.cs`の詳細解説
  - ミドルウェアパイプライン
  - ルーティング設定

#### Step 3: 実践理解
- **samples/calculator-app-explanation.md**
  - Data Annotationsの使い方
  - Model Bindingの仕組み
  - ViewModelパターン
  - サーバー/クライアント検証

#### Step 4: 実装練習
- **samples/README.md**
  - CalculatorアプリとBMIアプリの解説
  - 動作確認方法
  - 独自アプリ実装のヒント

### 3. データベース連携（所要時間: 4-5時間）

`docs/user-management/`配下でEF Core + SQLite + EAVモデルを学びます：

#### Step 1: 要件確認
- **user-management/requirements.md**
  - EAVモデルとは何か
  - 従来方式との違い
  - 機能要件の理解
  - データモデル設計

#### Step 2: DB設計理解
- **user-management/er-diagram.md**
  - 3テーブル構成（User/Attribute/UserAttributeValue）
  - リレーションシップ（1対多×2）
  - DbContext設定
  - 外部キー制約・カスケード削除

#### Step 3: 段階的実装
- **user-management/implementation-steps.md**
  - Step 1: 環境準備（NuGetパッケージ）
  - Step 2: Entity作成（3モデル）
  - Step 3: DbContext・マイグレーション実行
  - Step 4: 属性管理CRUD
  - Step 5-7: ユーザー管理CRUD + 動的フォーム
  - Step 8: UI改善・完成

## 📊 難易度マトリクス

| ドキュメント | 対象者 | 難易度 | 所要時間 |
|------------|--------|--------|---------|
| setup.md | 全員 | ★☆☆☆☆ | 10分 |
| themes.md | 全員 | ★☆☆☆☆ | 5分 |
| samples/mvc-for-beginners.md | MVC初学者 | ★★☆☆☆ | 30分 |
| samples/mvc_basics.md | 中級者 | ★★★☆☆ | 45分 |
| samples/calculator-app-explanation.md | 実装者 | ★★★☆☆ | 1時間 |
| samples/README.md | 実装者 | ★★☆☆☆ | 1時間 |
| user-management/requirements.md | DB学習者 | ★★★★☆ | 45分 |
| user-management/er-diagram.md | DB学習者 | ★★★★☆ | 1時間 |
| user-management/implementation-steps.md | 実装者 | ★★★★★ | 3-4時間 |

## 💡 Tips

### ドキュメントの読み方

1. **飛ばし読み禁止**: 基礎編は順番に読むことを推奨
2. **手を動かす**: コードを書きながら理解を深める
3. **質問を持つ**: なぜそうなるのか考えながら読む
4. **実験する**: ドキュメントの例を少し変えて試す

### 困ったときは

- **ビルドエラー**: `dotnet build` の出力を確認
- **実行エラー**: ブラウザのデベロッパーツールを確認
- **概念が不明**: `mvc-for-beginners.md` に戻る
- **実装に迷う**: `implementation-steps.md` の手順を再確認

## 🔄 更新履歴

- 2025/11/08: Areas構造に再編成、Todo実装ドキュメント追加
- 初版: MVC基礎学習ドキュメント作成

## 🚀 次のステップ

すべてのドキュメントを学習したら：

1. **独自機能の追加**: ユーザー検索機能、属性値でのフィルタリング
2. **Ajax対応**: Web API化してTypeScriptからアクセス
3. **認証機能**: ASP.NET Core Identity の学習
4. **フロントエンド**: React/Vue.jsとの連携

頑張ってください！🎉
