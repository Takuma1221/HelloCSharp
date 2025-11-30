import type { AttributeDefinition, AttributeFormData } from './types';

const API_BASE = '/api/UserManagement/AttributeApi';

/**
 * 属性API - fetchを使ったシンプルな実装
 */
export const attributeApi = {
    /**
     * 属性一覧を取得
     */
    async getAll(): Promise<AttributeDefinition[]> {
        const response = await fetch(API_BASE);
        if (!response.ok) throw new Error('属性の取得に失敗しました');
        return response.json();
    },

    /**
     * 属性を作成
     */
    async create(data: AttributeFormData): Promise<AttributeDefinition> {
        const response = await fetch(API_BASE, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '作成に失敗しました');
        }
        return response.json();
    },

    /**
     * 属性を更新
     */
    async update(id: number, data: AttributeFormData): Promise<AttributeDefinition> {
        const response = await fetch(`${API_BASE}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ ...data, id }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '更新に失敗しました');
        }
        return response.json();
    },

    /**
     * 属性を削除
     */
    async delete(id: number): Promise<void> {
        const response = await fetch(`${API_BASE}/${id}`, { method: 'DELETE' });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '削除に失敗しました');
        }
    },
};
