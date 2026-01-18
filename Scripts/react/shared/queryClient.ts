import { QueryClient } from '@tanstack/react-query';

/**
 * React Query の QueryClient 設定
 * - staleTime: データが古くなるまでの時間（5分）
 * - cacheTime: キャッシュの保持時間（10分）
 * - retry: エラー時のリトライ回数（1回のみ）
 */
export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 5 * 60 * 1000, // 5分
            gcTime: 10 * 60 * 1000, // 10分（v5からcacheTimeがgcTimeに変更）
            retry: 1,
            refetchOnWindowFocus: false,
        },
        mutations: {
            retry: 0,
        },
    },
});
