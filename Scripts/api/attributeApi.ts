import { ApiClient } from './apiClient.js';
import type { AttributeDefinition, AttributeRequest } from '../types/attribute.js';

/**
 * 属性API専用のクライアント
 */
export class AttributeApi {
    private client: ApiClient;
    private endpoint = '/api/UserManagement/AttributeApi';

    constructor() {
        this.client = new ApiClient();
    }

    /**
     * 属性一覧を取得
     */
    async getAll(): Promise<AttributeDefinition[]> {
        return this.client.get<AttributeDefinition[]>(this.endpoint);
    }

    /**
     * 特定の属性を取得
     */
    async getById(id: number): Promise<AttributeDefinition> {
        return this.client.get<AttributeDefinition>(`${this.endpoint}/${id}`);
    }

    /**
     * 新規属性を作成
     */
    async create(attribute: AttributeRequest): Promise<AttributeDefinition> {
        return this.client.post<AttributeDefinition>(this.endpoint, attribute);
    }

    /**
     * 属性を更新
     */
    async update(id: number, attribute: AttributeRequest): Promise<AttributeDefinition> {
        return this.client.put<AttributeDefinition>(`${this.endpoint}/${id}`, {
            ...attribute,
            id: id,
        });
    }

    /**
     * 属性を削除
     */
    async delete(id: number): Promise<void> {
        return this.client.delete(`${this.endpoint}/${id}`);
    }
}
