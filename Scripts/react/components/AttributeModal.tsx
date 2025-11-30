import React, { useState, useEffect } from 'react';
import type { AttributeDefinition, AttributeFormData } from '../types';

interface Props {
    isOpen: boolean;
    editingAttribute: AttributeDefinition | null;
    onClose: () => void;
    onSubmit: (data: AttributeFormData) => void;
}

const initialFormData: AttributeFormData = {
    attributeName: '',
    dataType: 'Text',
    displayOrder: 1,
    isRequired: false,
};

/**
 * 属性作成/編集モーダルコンポーネント
 */
export const AttributeModal: React.FC<Props> = ({
    isOpen,
    editingAttribute,
    onClose,
    onSubmit,
}) => {
    const [formData, setFormData] = useState<AttributeFormData>(initialFormData);

    // 編集時は既存データをセット
    useEffect(() => {
        if (editingAttribute) {
            setFormData({
                attributeName: editingAttribute.attributeName,
                dataType: editingAttribute.dataType,
                displayOrder: editingAttribute.displayOrder,
                isRequired: editingAttribute.isRequired,
            });
        } else {
            setFormData(initialFormData);
        }
    }, [editingAttribute, isOpen]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay show" onClick={onClose}>
            <div className="modal-dialog" onClick={(e) => e.stopPropagation()}>
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">
                            {editingAttribute ? '✏️ 属性編集' : '➕ 新規属性作成'}
                        </h5>
                        <button
                            type="button"
                            className="btn-close"
                            onClick={onClose}
                        />
                    </div>
                    <form onSubmit={handleSubmit}>
                        <div className="modal-body">
                            <div className="mb-3">
                                <label htmlFor="attributeName" className="form-label">
                                    属性名 <span className="text-danger">*</span>
                                </label>
                                <input
                                    type="text"
                                    className="form-control"
                                    id="attributeName"
                                    value={formData.attributeName}
                                    onChange={(e) =>
                                        setFormData({ ...formData, attributeName: e.target.value })
                                    }
                                    placeholder="例: 血液型"
                                    required
                                    maxLength={50}
                                />
                            </div>

                            <div className="mb-3">
                                <label htmlFor="dataType" className="form-label">
                                    データ型 <span className="text-danger">*</span>
                                </label>
                                <select
                                    className="form-select"
                                    id="dataType"
                                    value={formData.dataType}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            dataType: e.target.value as 'Text' | 'Number' | 'Date',
                                        })
                                    }
                                >
                                    <option value="Text">📝 文字列 (Text)</option>
                                    <option value="Number">🔢 数値 (Number)</option>
                                    <option value="Date">📅 日付 (Date)</option>
                                </select>
                            </div>

                            <div className="mb-3">
                                <label htmlFor="displayOrder" className="form-label">
                                    表示順
                                </label>
                                <input
                                    type="number"
                                    className="form-control"
                                    id="displayOrder"
                                    value={formData.displayOrder}
                                    onChange={(e) =>
                                        setFormData({
                                            ...formData,
                                            displayOrder: parseInt(e.target.value) || 1,
                                        })
                                    }
                                    min={1}
                                    max={999}
                                />
                            </div>

                            <div className="form-check mb-3">
                                <input
                                    type="checkbox"
                                    className="form-check-input"
                                    id="isRequired"
                                    checked={formData.isRequired}
                                    onChange={(e) =>
                                        setFormData({ ...formData, isRequired: e.target.checked })
                                    }
                                />
                                <label className="form-check-label" htmlFor="isRequired">
                                    必須項目にする
                                </label>
                            </div>
                        </div>

                        <div className="modal-footer">
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={onClose}
                            >
                                キャンセル
                            </button>
                            <button type="submit" className="btn btn-primary">
                                {editingAttribute ? '更新' : '作成'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};
