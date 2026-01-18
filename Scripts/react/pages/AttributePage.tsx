import React from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Provider as JotaiProvider, useAtom } from 'jotai';
import { AttributeTable } from '../components/AttributeTable';
import { AttributeModal } from '../components/AttributeModal';
import { ToastContainer, useToast } from '../components/Toast';
import { LoadingOverlay } from '../components/Loading';
import { queryClient } from '../shared/queryClient';
import {
    isModalOpenAtom,
    editingAttributeAtom,
    isLoadingAtom,
} from '../shared/atoms';
import {
    useAttributes,
    useCreateAttribute,
    useUpdateAttribute,
    useDeleteAttribute,
} from '../shared/queries';
import type { AttributeDefinition, AttributeFormData } from '../shared/types';

/**
 * 属性管理ページコンポーネント（Phase 3強化版）
 * - React Query: サーバーステート管理（キャッシュ、自動再取得）
 * - Jotai: クライアントステート管理（Atoms）
 * - TanStack Table: テーブル機能拡張（ソート、フィルタ、ページング）
 */
const AttributePageContent: React.FC = () => {
    // Jotai atoms
    const [isModalOpen, setIsModalOpen] = useAtom(isModalOpenAtom);
    const [editingAttribute, setEditingAttribute] = useAtom(editingAttributeAtom);
    const [isLoading, setIsLoading] = useAtom(isLoadingAtom);

    // React Query hooks
    const { data: attributes = [], isLoading: isQueryLoading } = useAttributes();
    const createMutation = useCreateAttribute();
    const updateMutation = useUpdateAttribute();
    const deleteMutation = useDeleteAttribute();

    // トースト通知
    const { messages, showSuccess, showError, removeToast } = useToast();

    // ローディング状態を統合
    const isAnyLoading =
        isLoading ||
        isQueryLoading ||
        createMutation.isPending ||
        updateMutation.isPending ||
        deleteMutation.isPending;

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
                await updateMutation.mutateAsync({
                    id: editingAttribute.id,
                    data,
                });
                showSuccess('属性を更新しました');
            } else {
                await createMutation.mutateAsync(data);
                showSuccess('属性を作成しました');
            }
            handleCloseModal();
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
            await deleteMutation.mutateAsync(id);
            showSuccess('属性を削除しました');
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
            <LoadingOverlay isLoading={isAnyLoading} />

            {/* ヘッダー */}
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>📋 属性管理</h2>
                <button className="btn btn-primary" onClick={handleCreate}>
                    ➕ 新規作成
                </button>
            </div>

            {/* 属性一覧テーブル（TanStack Table版） */}
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

/**
 * ルートコンポーネント（Providers）
 */
const AttributePage: React.FC = () => {
    return (
        <QueryClientProvider client={queryClient}>
            <JotaiProvider>
                <AttributePageContent />
            </JotaiProvider>
            {/* 開発用React Query DevTools */}
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    );
};

// DOMにマウント
const container = document.getElementById('react-root');
if (container) {
    const root = createRoot(container);
    root.render(<AttributePage />);
}
