import React from 'react';
import { createRoot } from 'react-dom/client';

/**
 * ホームページコンポーネント
 * EAVユーザー管理システムの概要と各機能へのリンクを表示
 */
const HomePage: React.FC = () => {
    return (
        <div className="row">
            <div className="col-lg-8 mx-auto">
                {/* ヘッダー */}
                <div className="text-center mb-5">
                    <h1 className="display-5">👥 EAVユーザー管理システム</h1>
                    <p className="lead text-muted">
                        Entity-Attribute-Value モデルを使った動的なユーザー管理
                    </p>
                </div>

                {/* アプリ説明 */}
                <div className="card mb-4">
                    <div className="card-header bg-primary text-white">
                        <h5 className="mb-0">📖 このアプリについて</h5>
                    </div>
                    <div className="card-body">
                        <p>
                            このアプリは <strong>EAV（Entity-Attribute-Value）モデル</strong> を学習するために作成されたサンプルです。
                            会社のシステムで使われている <code>user_attribute_value</code> のような構造を、
                            ASP.NET Core + React で実装しています。
                        </p>
                        <p className="mb-0">
                            ユーザーに紐づく属性（年齢、部署、役職など）を動的に追加・管理できます。
                        </p>
                    </div>
                </div>

                {/* 機能リンク */}
                <div className="row g-4 mb-4">
                    <div className="col-md-6">
                        <div className="card h-100 border-primary">
                            <div className="card-body">
                                <h5 className="card-title">📋 属性管理</h5>
                                <p className="card-text text-muted">
                                    ユーザーに設定できる属性（項目）を定義します。<br />
                                    データ型（文字列/数値/日付）や必須設定が可能です。
                                </p>
                                <a href="/UserManagement/Attribute" className="btn btn-primary">
                                    属性管理へ →
                                </a>
                            </div>
                        </div>
                    </div>
                    <div className="col-md-6">
                        <div className="card h-100 border-success">
                            <div className="card-body">
                                <h5 className="card-title">👤 ユーザー管理</h5>
                                <p className="card-text text-muted">
                                    ユーザーを登録し、定義した属性の値を設定します。<br />
                                    動的なフォームで柔軟に管理できます。
                                </p>
                                <a href="/UserManagement/User" className="btn btn-success">
                                    ユーザー管理へ →
                                </a>
                            </div>
                        </div>
                    </div>
                </div>

                {/* 技術スタック */}
                <div className="card">
                    <div className="card-header">
                        <h5 className="mb-0">🛠️ 技術スタック</h5>
                    </div>
                    <div className="card-body">
                        <div className="row">
                            <div className="col-md-6">
                                <h6>バックエンド</h6>
                                <ul className="list-unstyled">
                                    <li>✅ ASP.NET Core MVC</li>
                                    <li>✅ Raw SQL (SQLite)</li>
                                    <li>✅ Web API (RESTful)</li>
                                </ul>
                            </div>
                            <div className="col-md-6">
                                <h6>フロントエンド</h6>
                                <ul className="list-unstyled">
                                    <li>✅ React 19</li>
                                    <li>✅ TypeScript</li>
                                    <li>✅ Bootstrap 5</li>
                                    <li>✅ esbuild</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

// DOMにマウント
const container = document.getElementById('react-root');
if (container) {
    const root = createRoot(container);
    root.render(<HomePage />);
}
