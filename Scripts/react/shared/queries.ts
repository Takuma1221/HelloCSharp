import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { attributeApi } from './api';
import type { AttributeFormData } from './types';

/**
 * 属性一覧を取得するクエリ
 */
export const useAttributes = () => {
    return useQuery({
        queryKey: ['attributes'],
        queryFn: attributeApi.getAll,
    });
};

/**
 * 属性を作成するミューテーション
 */
export const useCreateAttribute = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: AttributeFormData) => attributeApi.create(data),
        onSuccess: () => {
            // 成功時に一覧を再取得
            queryClient.invalidateQueries({ queryKey: ['attributes'] });
        },
    });
};

/**
 * 属性を更新するミューテーション
 */
export const useUpdateAttribute = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: number; data: AttributeFormData }) =>
            attributeApi.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['attributes'] });
        },
    });
};

/**
 * 属性を削除するミューテーション
 */
export const useDeleteAttribute = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: number) => attributeApi.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['attributes'] });
        },
    });
};
