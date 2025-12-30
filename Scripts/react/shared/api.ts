import type { AttributeDefinition, AttributeFormData, User, UserFormData, UserAttributeValue } from './types';

// 生SQL版のエンドポイント
const ATTRIBUTE_API_BASE = '/api/UserManagement/AttributeSql';
const USER_API_BASE = '/api/UserManagement/UserSql';

/**
 * 属性API - fetchを使ったシンプルな実装
 */
export const attributeApi = {
    /**
     * 属性一覧を取得
     */
    async getAll(): Promise<AttributeDefinition[]> {
        const response = await fetch(ATTRIBUTE_API_BASE);
        if (!response.ok) throw new Error('属性の取得に失敗しました');
        return response.json();
    },

    /**
     * 属性を作成
     */
    async create(data: AttributeFormData): Promise<AttributeDefinition> {
        const response = await fetch(ATTRIBUTE_API_BASE, {
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
        const response = await fetch(`${ATTRIBUTE_API_BASE}/${id}`, {
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
        const response = await fetch(`${ATTRIBUTE_API_BASE}/${id}`, { method: 'DELETE' });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '削除に失敗しました');
        }
    },
};

/**
 * ユーザーAPI
 */
export const userApi = {
    /**
     * ユーザー一覧を取得
     */
    async getAll(): Promise<User[]> {
        const response = await fetch(USER_API_BASE);
        if (!response.ok) throw new Error('ユーザーの取得に失敗しました');
        return response.json();
    },

    /**
     * ユーザーを作成
     */
    async create(data: UserFormData): Promise<User> {
        const response = await fetch(USER_API_BASE, {
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
     * ユーザーを更新
     */
    async update(id: number, data: UserFormData): Promise<User> {
        const response = await fetch(`${USER_API_BASE}/${id}`, {
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
     * ユーザーを削除
     */
    async delete(id: number): Promise<void> {
        const response = await fetch(`${USER_API_BASE}/${id}`, { method: 'DELETE' });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '削除に失敗しました');
        }
    },
};

/**
 * ユーザー属性値API
 */
export const userAttributeValueApi = {
    /**
     * 特定ユーザーの属性値を取得
     */
    async getByUserId(userId: number): Promise<UserAttributeValue[]> {
        const response = await fetch(`/api/UserManagement/UserAttributeValue/user/${userId}`);
        if (!response.ok) throw new Error('属性値の取得に失敗しました');
        return response.json();
    },

    /**
     * ユーザーの属性値を保存
     */
    async save(userId: number, attributeValues: Record<number, string>): Promise<void> {
        const response = await fetch(`/api/UserManagement/UserAttributeValue/user/${userId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(attributeValues),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || '保存に失敗しました');
        }
    },
};
