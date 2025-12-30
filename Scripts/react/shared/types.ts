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
