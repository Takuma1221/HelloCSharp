import React, { useState, useCallback, useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import { userApi, attributeApi, userAttributeValueApi } from '../shared/api';
import type { User, UserFormData, AttributeDefinition, UserAttributeValue } from '../shared/types';

/**
 * ユーザー管理ページ
 */
const UserPage: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [attributes, setAttributes] = useState<AttributeDefinition[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showModal, setShowModal] = useState(false);
    const [editingUser, setEditingUser] = useState<User | null>(null);
    const [formData, setFormData] = useState<UserFormData>({
        name: '',
        email: '',
    });
    const [attributeValues, setAttributeValues] = useState<Record<number, string>>({});

    // 初期データ取得
    const fetchData = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const [usersData, attributesData] = await Promise.all([
                userApi.getAll(),
                attributeApi.getAll(),
            ]);
            setUsers(usersData);
            setAttributes(attributesData);
        } catch (err) {
            setError(err instanceof Error ? err.message : '取得に失敗しました');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    // モーダルを開く
    const openModal = async (user?: User) => {
        if (user) {
            setEditingUser(user);
            setFormData({
                name: user.name,
                email: user.email,
            });
            
            // 既存の属性値を取得
            try {
                const values = await userAttributeValueApi.getByUserId(user.id);
                const valuesMap: Record<number, string> = {};
                values.forEach((v: UserAttributeValue) => {
                    valuesMap[v.attributeId] = v.value;
                });
                setAttributeValues(valuesMap);
            } catch (err) {
                console.error('属性値の取得に失敗:', err);
                setAttributeValues({});
            }
        } else {
            setEditingUser(null);
            setFormData({
                name: '',
                email: '',
            });
            setAttributeValues({});
        }
        setShowModal(true);
    };

    // モーダルを閉じる
    const closeModal = () => {
        setShowModal(false);
        setEditingUser(null);
        setFormData({
            name: '',
            email: '',
        });
        setAttributeValues({});
    };

    // フォーム送信
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            let userId: number;
            
            if (editingUser) {
                await userApi.update(editingUser.id, formData);
                userId = editingUser.id;
            } else {
                const created = await userApi.create(formData);
                userId = created.id;
            }
            
            // 属性値を保存
            await userAttributeValueApi.save(userId, attributeValues);
            
            closeModal();
            fetchData();
        } catch (err) {
            alert(err instanceof Error ? err.message : '保存に失敗しました');
        }
    };

    // 削除
    const handleDelete = async (user: User) => {
        if (!confirm(`「${user.name}」を削除してもよろしいですか？\n関連する属性値も削除されます。`)) {
            return;
        }
        try {
            await userApi.delete(user.id);
            fetchData();
        } catch (err) {
            alert(err instanceof Error ? err.message : '削除に失敗しました');
        }
    };

    // 属性値の入力変更
    const handleAttributeChange = (attributeId: number, value: string) => {
        setAttributeValues({
            ...attributeValues,
            [attributeId]: value,
        });
    };

    // データ型に応じた入力フィールドを生成
    const renderAttributeInput = (attr: AttributeDefinition) => {
        const value = attributeValues[attr.id] || '';
        
        switch (attr.dataType) {
            case 'Number':
                return (
                    <input
                        type="number"
                        className="form-control"
                        value={value}
                        onChange={(e) => handleAttributeChange(attr.id, e.target.value)}
                        required={attr.isRequired}
                    />
                );
            case 'Date':
                return (
                    <input
                        type="date"
                        className="form-control"
                        value={value}
                        onChange={(e) => handleAttributeChange(attr.id, e.target.value)}
                        required={attr.isRequired}
                    />
                );
            case 'Text':
            default:
                return (
                    <input
                        type="text"
                        className="form-control"
                        value={value}
                        onChange={(e) => handleAttributeChange(attr.id, e.target.value)}
                        required={attr.isRequired}
                    />
                );
        }
    };

    if (loading) {
        return (
            <div className="text-center py-5">
                <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    return (
        <div className="container mt-4">
            {/* ヘッダー */}
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h2>👥 ユーザー管理</h2>
                    <p className="text-muted mb-0">
                        ユーザーの登録・編集・削除ができます
                    </p>
                </div>
                <button
                    onClick={() => openModal()}
                    className="btn btn-primary"
                >
                    ➕ 新規ユーザー
                </button>
            </div>

            {/* エラー表示 */}
            {error && (
                <div className="alert alert-danger" role="alert">
                    {error}
                </div>
            )}

            {/* ユーザー一覧テーブル */}
            <div className="card">
                <div className="card-body">
                    {users.length === 0 ? (
                        <p className="text-center text-muted my-4">
                            ユーザーが登録されていません
                        </p>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-hover">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>名前</th>
                                        <th>メールアドレス</th>
                                        <th>登録日</th>
                                        <th style={{ width: '150px' }}>操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {users.map((user) => (
                                        <tr key={user.id}>
                                            <td>{user.id}</td>
                                            <td>{user.name}</td>
                                            <td>{user.email}</td>
                                            <td>
                                                {new Date(user.createdAt).toLocaleDateString('ja-JP')}
                                            </td>
                                            <td>
                                                <button
                                                    onClick={() => openModal(user)}
                                                    className="btn btn-sm btn-outline-primary me-2"
                                                >
                                                    編集
                                                </button>
                                                <button
                                                    onClick={() => handleDelete(user)}
                                                    className="btn btn-sm btn-outline-danger"
                                                >
                                                    削除
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>

            {/* モーダル */}
            {showModal && (
                <div className="modal show d-block" tabIndex={-1}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <form onSubmit={handleSubmit}>
                                <div className="modal-header">
                                    <h5 className="modal-title">
                                        {editingUser ? 'ユーザー編集' : '新規ユーザー'}
                                    </h5>
                                    <button
                                        type="button"
                                        className="btn-close"
                                        onClick={closeModal}
                                    ></button>
                                </div>
                                <div className="modal-body">
                                    <div className="mb-3">
                                        <label className="form-label">名前 *</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.name}
                                            onChange={(e) =>
                                                setFormData({ ...formData, name: e.target.value })
                                            }
                                            required
                                            maxLength={100}
                                        />
                                    </div>
                                    <div className="mb-3">
                                        <label className="form-label">メールアドレス *</label>
                                        <input
                                            type="email"
                                            className="form-control"
                                            value={formData.email}
                                            onChange={(e) =>
                                                setFormData({ ...formData, email: e.target.value })
                                            }
                                            required
                                        />
                                    </div>
                                    
                                    {/* 動的属性フィールド */}
                                    {attributes.length > 0 && (
                                        <>
                                            <hr />
                                            <h6 className="mb-3">📝 ユーザー属性</h6>
                                            {attributes.map((attr) => (
                                                <div key={attr.id} className="mb-3">
                                                    <label className="form-label">
                                                        {attr.attributeName}
                                                        {attr.isRequired && <span className="text-danger"> *</span>}
                                                        <small className="text-muted ms-2">
                                                            ({attr.dataType === 'Text' ? 'テキスト' : 
                                                              attr.dataType === 'Number' ? '数値' : '日付'})
                                                        </small>
                                                    </label>
                                                    {renderAttributeInput(attr)}
                                                </div>
                                            ))}
                                        </>
                                    )}
                                </div>
                                <div className="modal-footer">
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        onClick={closeModal}
                                    >
                                        キャンセル
                                    </button>
                                    <button type="submit" className="btn btn-primary">
                                        {editingUser ? '更新' : '作成'}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
            {showModal && <div className="modal-backdrop show"></div>}
        </div>
    );
};

// DOMにマウント
const container = document.getElementById('react-root');
if (container) {
    const root = createRoot(container);
    root.render(<UserPage />);
}
