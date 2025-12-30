/**
 * 属性定義の型（APIレスポンスの形式）
 */
export interface AttributeDefinition {
    id: number;
    attributeName: string;
    dataType: 'Text' | 'Number' | 'Date';
    displayOrder: number;
    isRequired: boolean;
    createdAt: string;
}

/**
 * 属性作成/更新リクエストの型
 */
export interface AttributeFormData {
    attributeName: string;
    dataType: 'Text' | 'Number' | 'Date';
    displayOrder: number;
    isRequired: boolean;
}

/**
 * APIエラーレスポンスの型
 */
export interface ApiError {
    message: string;
    id?: number;
    usageCount?: number;
}

/**
 * ユーザーの型（APIレスポンスの形式）
 */
export interface User {
    id: number;
    name: string;
    email: string;
    createdAt: string;
    updatedAt: string;
}

/**
 * ユーザー作成/更新リクエストの型
 */
export interface UserFormData {
    name: string;
    email: string;
}

/**
 * ユーザー属性値の型
 */
export interface UserAttributeValue {
    id: number;
    userId: number;
    attributeId: number;
    value: string;
    createdAt: string;
    updatedAt: string;
}

/**
 * ユーザー詳細（属性値含む）
 */
export interface UserWithAttributes extends User {
    attributeValues: Record<number, string>; // { attributeId: value }
}
