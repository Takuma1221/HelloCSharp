/**
 * API通信を行う汎用クライアント
 */
export class ApiClient {
    private baseUrl: string;

    constructor(baseUrl: string = '') {
        this.baseUrl = baseUrl;
    }

    /**
     * GETリクエスト
     */
    async get<T>(endpoint: string): Promise<T> {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
            },
        });

        if (!response.ok) {
            throw await this.handleError(response);
        }

        return response.json();
    }

    /**
     * POSTリクエスト
     */
    async post<T>(endpoint: string, data: unknown): Promise<T> {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            throw await this.handleError(response);
        }

        return response.json();
    }

    /**
     * PUTリクエスト
     */
    async put<T>(endpoint: string, data: unknown): Promise<T> {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            throw await this.handleError(response);
        }

        return response.json();
    }

    /**
     * DELETEリクエスト
     */
    async delete(endpoint: string): Promise<void> {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'DELETE',
            headers: {
                'Accept': 'application/json',
            },
        });

        if (!response.ok) {
            throw await this.handleError(response);
        }

        // 204 No Content の場合はボディがない
    }

    /**
     * エラーハンドリング
     */
    private async handleError(response: Response): Promise<Error> {
        let errorMessage = `HTTP Error: ${response.status}`;
        
        try {
            const errorData = await response.json();
            if (errorData.message) {
                errorMessage = errorData.message;
            }
        } catch {
            // JSONパースに失敗した場合はデフォルトメッセージを使用
        }

        return new Error(errorMessage);
    }
}
