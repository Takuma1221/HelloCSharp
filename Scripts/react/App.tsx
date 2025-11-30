import React, { useState, useEffect, useCallback } from 'react';
import { createRoot } from 'react-dom/client';
import { AttributeTable } from './components/AttributeTable';
import { AttributeModal } from './components/AttributeModal';
import { ToastContainer, useToast } from './components/Toast';
import { LoadingOverlay } from './components/Loading';
import { attributeApi } from './api';
import type { AttributeDefinition, AttributeFormData } from './types';

/**
 * 属性管理アプリケーションのメインコンポーネント
 * すべての状態管理とイベント処理を担当
 */
const AttributeApp: React.FC = () => {
    // 状態管理
    const [attributes, setAttributes] = useState<AttributeDefinition[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingAttribute, setEditingAttribute] = useState<AttributeDefinition | null>(null);
    
    // トースト通知
    const { messages, showSuccess, showError, removeToast } = useToast();

    /**
     * 属性一覧を読み込み
     */
    const loadAttributes = useCallback(async () => {
        try {
            setIsLoading(true);
            const data = await attributeApi.getAll();
            setAttributes(data);
        } catch (error) {
            showError((error as Error).message);
        } finally {
            setIsLoading(false);
        }
    }, []);

    // 初回読み込み
    useEffect(() => {
        loadAttributes();
    }, [loadAttributes]);

    /**
     * 新規作成モーダルを開く
     */
    const handleCreate = () => {
        setEditingAttribute(null);
        setIsModalOpen(true);
    };

    /**
     * 編集モーダルを開く
     */
    const handleEdit = (attr: AttributeDefinition) => {
        setEditingAttribute(attr);
        setIsModalOpen(true);
    };

    /**
     * モーダルを閉じる
     */
    const handleCloseModal = () => {
        setIsModalOpen(false);
        setEditingAttribute(null);
    };

    /**
     * フォーム送信処理
     */
    const handleSubmit = async (data: AttributeFormData) => {
        try {
            setIsLoading(true);
            if (editingAttribute) {
                await attributeApi.update(editingAttribute.id, data);
                showSuccess('属性を更新しました');
            } else {
                await attributeApi.create(data);
                showSuccess('属性を作成しました');
            }
            handleCloseModal();
            await loadAttributes();
        } catch (error) {
            showError((error as Error).message);
        } finally {
            setIsLoading(false);
        }
    };

    /**
     * 削除処理
     */
    const handleDelete = async (id: number) => {
        const attr = attributes.find((a) => a.id === id);
        if (!attr) return;

        if (!window.confirm(`「${attr.attributeName}」を削除してもよろしいですか？`)) {
            return;
        }

        try {
            setIsLoading(true);
            await attributeApi.delete(id);
            showSuccess('属性を削除しました');
            await loadAttributes();
        } catch (error) {
            showError((error as Error).message);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <>
            {/* トースト通知 */}
            <ToastContainer messages={messages} onRemove={removeToast} />

            {/* ローディング */}
            <LoadingOverlay isLoading={isLoading} />

            {/* ヘッダー */}
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>📋 属性管理</h2>
                <button className="btn btn-primary" onClick={handleCreate}>
                    ➕ 新規作成
                </button>
            </div>

            {/* 属性一覧テーブル */}
            <AttributeTable
                attributes={attributes}
                onEdit={handleEdit}
                onDelete={handleDelete}
            />

            {/* 作成/編集モーダル */}
            <AttributeModal
                isOpen={isModalOpen}
                editingAttribute={editingAttribute}
                onClose={handleCloseModal}
                onSubmit={handleSubmit}
            />
        </>
    );
};

// DOMにマウント
const container = document.getElementById('react-root');
if (container) {
    const root = createRoot(container);
    root.render(<AttributeApp />);
}
