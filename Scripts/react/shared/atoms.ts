import { atom } from 'jotai';
import type { AttributeDefinition } from './types';

/**
 * モーダル開閉状態
 */
export const isModalOpenAtom = atom(false);

/**
 * 編集中の属性（nullの場合は新規作成）
 */
export const editingAttributeAtom = atom<AttributeDefinition | null>(null);

/**
 * ローディング状態（API呼び出し中）
 */
export const isLoadingAtom = atom(false);
