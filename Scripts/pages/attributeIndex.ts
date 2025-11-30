import { AttributeApi } from '../api/attributeApi.js';
import type { AttributeDefinition, AttributeRequest } from '../types/attribute.js';

/**
 * 属性管理画面のメインクラス
 * 一覧表示・CRUD操作をすべてTypeScriptで制御
 */
class AttributeManager {
    private api: AttributeApi;
    private attributes: AttributeDefinition[] = [];
    private editingId: number | null = null;

    // DOM要素
    private tableBody: HTMLTableSectionElement | null = null;
    private modal: HTMLElement | null = null;
    private form: HTMLFormElement | null = null;
    private modalTitle: HTMLElement | null = null;
    private loadingOverlay: HTMLElement | null = null;

    constructor() {
        this.api = new AttributeApi();
    }

    /**
     * 初期化処理
     */
    async init(): Promise<void> {
        this.bindElements();
        this.bindEvents();
        await this.loadAttributes();
    }

    /**
     * DOM要素をバインド
     */
    private bindElements(): void {
        this.tableBody = document.getElementById('attribute-table-body') as HTMLTableSectionElement;
        this.modal = document.getElementById('attribute-modal');
        this.form = document.getElementById('attribute-form') as HTMLFormElement;
        this.modalTitle = document.getElementById('modal-title');
        this.loadingOverlay = document.getElementById('loading-overlay');
    }

    /**
     * イベントリスナーを設定
     */
    private bindEvents(): void {
        // 新規作成ボタン
        document.getElementById('btn-create')?.addEventListener('click', () => {
            this.openCreateModal();
        });

        // フォーム送信
        this.form?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleSubmit();
        });

        // モーダルキャンセル
        document.getElementById('btn-cancel')?.addEventListener('click', () => {
            this.closeModal();
        });

        // モーダル外クリックで閉じる
        this.modal?.addEventListener('click', (e) => {
            if (e.target === this.modal) {
                this.closeModal();
            }
        });
    }

    /**
     * 属性一覧を読み込み
     */
    private async loadAttributes(): Promise<void> {
        this.showLoading(true);

        try {
            this.attributes = await this.api.getAll();
            this.renderTable();
        } catch (error) {
            this.showError('属性の読み込みに失敗しました: ' + (error as Error).message);
        } finally {
            this.showLoading(false);
        }
    }

    /**
     * テーブルを描画
     */
    private renderTable(): void {
        if (!this.tableBody) return;

        if (this.attributes.length === 0) {
            this.tableBody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-muted py-4">
                        属性が登録されていません
                    </td>
                </tr>
            `;
            return;
        }

        this.tableBody.innerHTML = this.attributes.map(attr => `
            <tr data-id="${attr.id}">
                <td>${attr.id}</td>
                <td>${this.escapeHtml(attr.attributeName)}</td>
                <td><span class="badge ${this.getDataTypeBadgeClass(attr.dataType)}">${attr.dataType}</span></td>
                <td>${attr.displayOrder}</td>
                <td>${attr.isRequired ? '✅' : '❌'}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-edit" data-id="${attr.id}">
                        編集
                    </button>
                    <button class="btn btn-sm btn-outline-danger btn-delete" data-id="${attr.id}">
                        削除
                    </button>
                </td>
            </tr>
        `).join('');

        // 編集・削除ボタンのイベントを設定
        this.tableBody.querySelectorAll('.btn-edit').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const id = parseInt((e.target as HTMLElement).dataset.id || '0');
                this.openEditModal(id);
            });
        });

        this.tableBody.querySelectorAll('.btn-delete').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const id = parseInt((e.target as HTMLElement).dataset.id || '0');
                this.handleDelete(id);
            });
        });
    }

    /**
     * データ型に応じたバッジクラスを返す
     */
    private getDataTypeBadgeClass(dataType: string): string {
        switch (dataType) {
            case 'Text': return 'bg-primary';
            case 'Number': return 'bg-success';
            case 'Date': return 'bg-info';
            default: return 'bg-secondary';
        }
    }

    /**
     * 新規作成モーダルを開く
     */
    private openCreateModal(): void {
        this.editingId = null;
        if (this.modalTitle) this.modalTitle.textContent = '➕ 新規属性作成';
        this.form?.reset();
        this.showModal();
    }

    /**
     * 編集モーダルを開く
     */
    private openEditModal(id: number): void {
        const attr = this.attributes.find(a => a.id === id);
        if (!attr) return;

        this.editingId = id;
        if (this.modalTitle) this.modalTitle.textContent = '✏️ 属性編集';

        // フォームに値をセット
        const form = this.form;
        if (form) {
            (form.querySelector('[name="attributeName"]') as HTMLInputElement).value = attr.attributeName;
            (form.querySelector('[name="dataType"]') as HTMLSelectElement).value = attr.dataType;
            (form.querySelector('[name="displayOrder"]') as HTMLInputElement).value = attr.displayOrder.toString();
            (form.querySelector('[name="isRequired"]') as HTMLInputElement).checked = attr.isRequired;
        }

        this.showModal();
    }

    /**
     * フォーム送信処理
     */
    private async handleSubmit(): Promise<void> {
        if (!this.form) return;

        const formData = new FormData(this.form);
        const request: AttributeRequest = {
            attributeName: formData.get('attributeName') as string,
            dataType: formData.get('dataType') as string,
            displayOrder: parseInt(formData.get('displayOrder') as string),
            isRequired: formData.get('isRequired') === 'on',
        };

        this.showLoading(true);

        try {
            if (this.editingId) {
                // 更新
                await this.api.update(this.editingId, request);
                this.showSuccess('属性を更新しました');
            } else {
                // 新規作成
                await this.api.create(request);
                this.showSuccess('属性を作成しました');
            }

            this.closeModal();
            await this.loadAttributes();
        } catch (error) {
            this.showError('保存に失敗しました: ' + (error as Error).message);
        } finally {
            this.showLoading(false);
        }
    }

    /**
     * 削除処理
     */
    private async handleDelete(id: number): Promise<void> {
        const attr = this.attributes.find(a => a.id === id);
        if (!attr) return;

        if (!confirm(`「${attr.attributeName}」を削除してもよろしいですか？`)) {
            return;
        }

        this.showLoading(true);

        try {
            await this.api.delete(id);
            this.showSuccess('属性を削除しました');
            await this.loadAttributes();
        } catch (error) {
            this.showError('削除に失敗しました: ' + (error as Error).message);
        } finally {
            this.showLoading(false);
        }
    }

    /**
     * モーダルを表示
     */
    private showModal(): void {
        this.modal?.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    /**
     * モーダルを閉じる
     */
    private closeModal(): void {
        this.modal?.classList.remove('show');
        document.body.style.overflow = '';
    }

    /**
     * ローディング表示
     */
    private showLoading(show: boolean): void {
        if (this.loadingOverlay) {
            this.loadingOverlay.style.display = show ? 'flex' : 'none';
        }
    }

    /**
     * 成功メッセージ表示
     */
    private showSuccess(message: string): void {
        this.showToast(message, 'success');
    }

    /**
     * エラーメッセージ表示
     */
    private showError(message: string): void {
        this.showToast(message, 'danger');
    }

    /**
     * トースト通知を表示
     */
    private showToast(message: string, type: 'success' | 'danger'): void {
        const container = document.getElementById('toast-container');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = `toast show align-items-center text-white bg-${type} border-0`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${this.escapeHtml(message)}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;

        container.appendChild(toast);

        // 閉じるボタン
        toast.querySelector('.btn-close')?.addEventListener('click', () => {
            toast.remove();
        });

        // 3秒後に自動で消える
        setTimeout(() => {
            toast.remove();
        }, 3000);
    }

    /**
     * HTMLエスケープ
     */
    private escapeHtml(text: string): string {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// ページ読み込み完了時に初期化
document.addEventListener('DOMContentLoaded', () => {
    const manager = new AttributeManager();
    manager.init();
});
