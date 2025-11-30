import React from 'react';
import type { AttributeDefinition } from '../types';

interface Props {
    attributes: AttributeDefinition[];
    onEdit: (attr: AttributeDefinition) => void;
    onDelete: (id: number) => void;
}

/**
 * データ型に応じたバッジの色を返す
 */
const getDataTypeBadge = (dataType: string) => {
    const colors: Record<string, string> = {
        Text: 'bg-primary',
        Number: 'bg-success',
        Date: 'bg-info',
    };
    return colors[dataType] || 'bg-secondary';
};

/**
 * 属性一覧テーブルコンポーネント
 */
export const AttributeTable: React.FC<Props> = ({ attributes, onEdit, onDelete }) => {
    if (attributes.length === 0) {
        return (
            <div className="text-center text-muted py-5">
                <p>📭 属性が登録されていません</p>
                <p className="small">「新規作成」ボタンから属性を追加してください</p>
            </div>
        );
    }

    return (
        <div className="table-responsive">
            <table className="table table-striped table-hover">
                <thead className="table-dark">
                    <tr>
                        <th>ID</th>
                        <th>属性名</th>
                        <th>データ型</th>
                        <th>表示順</th>
                        <th>必須</th>
                        <th>操作</th>
                    </tr>
                </thead>
                <tbody>
                    {attributes.map((attr) => (
                        <tr key={attr.id}>
                            <td>{attr.id}</td>
                            <td>{attr.attributeName}</td>
                            <td>
                                <span className={`badge ${getDataTypeBadge(attr.dataType)}`}>
                                    {attr.dataType}
                                </span>
                            </td>
                            <td>{attr.displayOrder}</td>
                            <td>{attr.isRequired ? '✅' : '❌'}</td>
                            <td>
                                <button
                                    className="btn btn-sm btn-outline-primary me-2"
                                    onClick={() => onEdit(attr)}
                                >
                                    ✏️ 編集
                                </button>
                                <button
                                    className="btn btn-sm btn-outline-danger"
                                    onClick={() => onDelete(attr.id)}
                                >
                                    🗑️ 削除
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};
