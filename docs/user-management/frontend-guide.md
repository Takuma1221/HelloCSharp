# フロントエンド実装ガイド - TypeScript Web API版

## 🎯 概要

このガイドでは、既存のASP.NET Core MVCアプリに**TypeScriptとWeb API**を追加して、
会社で使っている`APIClient.ts`と同じ形式のフロントエンド実装を学びます。

## 📋 実装フロー

```
Phase 1: Web API追加（バックエンド）
  ↓
Phase 2: TypeScript環境構築
  ↓
Phase 3: APIClient.ts作成
  ↓
Phase 4: RazorビューからAjax呼び出し
  ↓
Phase 5: 動的UI更新
```

## Phase 1: Web API Controller追加（30分）

### 1-1. UserApiController作成

**ファイル**: `Areas/UserManagement/Controllers/Api/UserApiController.cs`

（先ほど提示したコードを使用）

### 1-2. AttributeApiController作成

**ファイル**: `Areas/UserManagement/Controllers/Api/AttributeApiController.cs`

```csharp
using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

[Route("api/attributes")]
[ApiController]
public class AttributeApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttributeApiController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttributeDefinition>>> GetAttributes()
    {
        return await _context.Attributes
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }
}
```

## Phase 2: TypeScript環境構築（15分）

### 2-1. Node.js初期化

```bash
# プロジェクトルートで実行
npm init -y
npm install --save-dev typescript @types/node
```

### 2-2. tsconfig.json作成

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ES2020",
    "outDir": "./wwwroot/js",
    "rootDir": "./wwwroot/ts",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true
  },
  "include": ["wwwroot/ts/**/*"]
}
```

### 2-3. package.jsonにスクリプト追加

```json
{
  "scripts": {
    "build:ts": "tsc",
    "watch:ts": "tsc --watch"
  }
}
```

### 2-4. フォルダ作成

```bash
mkdir -p wwwroot/ts
```

## Phase 3: APIClient.ts作成（30分）

### 3-1. 型定義

**ファイル**: `wwwroot/ts/types.ts`

```typescript
export interface User {
    id: number;
    name: string;
    email: string;
    createdAt: string;
    attributes: AttributeValue[];
}

export interface AttributeValue {
    attributeName: string;
    value: string;
    dataType: string;
}

export interface AttributeDefinition {
    id: number;
    attributeName: string;
    dataType: string;
    displayOrder: number;
    isRequired: boolean;
}

export interface CreateUserRequest {
    name: string;
    email: string;
    attributeValues?: AttributeValueInput[];
}

export interface AttributeValueInput {
    attributeId: number;
    value: string;
}
```

### 3-2. UserApiClient

**ファイル**: `wwwroot/ts/userApiClient.ts`

```typescript
import type { User, CreateUserRequest } from './types.js';

export class UserApiClient {
    private baseUrl = '/api/users';

    async getAll(): Promise<User[]> {
        const response = await fetch(this.baseUrl);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    }

    async getById(id: number): Promise<User> {
        const response = await fetch(`${this.baseUrl}/${id}`);
        if (!response.ok) {
            throw new Error('User not found');
        }
        return await response.json();
    }

    async create(data: CreateUserRequest): Promise<User> {
        const response = await fetch(this.baseUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(`Failed to create user: ${error}`);
        }
        return await response.json();
    }

    async update(id: number, data: CreateUserRequest): Promise<void> {
        const response = await fetch(`${this.baseUrl}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            throw new Error('Failed to update user');
        }
    }

    async delete(id: number): Promise<void> {
        const response = await fetch(`${this.baseUrl}/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            throw new Error('Failed to delete user');
        }
    }
}
```

### 3-3. AttributeApiClient

**ファイル**: `wwwroot/ts/attributeApiClient.ts`

```typescript
import type { AttributeDefinition } from './types.js';

export class AttributeApiClient {
    private baseUrl = '/api/attributes';

    async getAll(): Promise<AttributeDefinition[]> {
        const response = await fetch(this.baseUrl);
        if (!response.ok) {
            throw new Error('Failed to fetch attributes');
        }
        return await response.json();
    }
}
```

### 3-4. ビルド実行

```bash
npm run build:ts
```

→ `wwwroot/js/` 配下にJSファイルが生成されます

## Phase 4: Razorビューから使用（45分）

### 4-1. ユーザー一覧（Ajax版）

**ファイル**: `Areas/UserManagement/Views/User/IndexApi.cshtml`

```html
@{
    ViewData["Title"] = "ユーザー一覧（API版）";
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h2>👥 @ViewData["Title"]</h2>
    <button id="btnReload" class="btn btn-primary">🔄 再読み込み</button>
</div>

<div id="userList">
    <div class="text-center">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
</div>

<script type="module">
    import { UserApiClient } from '/js/userApiClient.js';

    const apiClient = new UserApiClient();
    const userListEl = document.getElementById('userList');

    async function loadUsers() {
        try {
            userListEl.innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
            
            const users = await apiClient.getAll();
            
            if (users.length === 0) {
                userListEl.innerHTML = `
                    <div class="alert alert-info">
                        ユーザーが登録されていません。
                    </div>
                `;
                return;
            }

            userListEl.innerHTML = users.map(user => `
                <div class="card mb-3" id="user-${user.id}">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h5 class="card-title">${escapeHtml(user.name)}</h5>
                                <p class="card-text text-muted">${escapeHtml(user.email)}</p>
                                ${user.attributes.length > 0 ? `
                                    <dl class="row mb-0">
                                        ${user.attributes.map(attr => `
                                            <dt class="col-sm-3">${escapeHtml(attr.attributeName)}</dt>
                                            <dd class="col-sm-9">${escapeHtml(attr.value)}</dd>
                                        `).join('')}
                                    </dl>
                                ` : ''}
                            </div>
                            <div>
                                <button class="btn btn-sm btn-outline-danger" onclick="deleteUser(${user.id})">
                                    削除
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `).join('');
        } catch (error) {
            userListEl.innerHTML = `
                <div class="alert alert-danger">
                    エラーが発生しました: ${escapeHtml(error.message)}
                </div>
            `;
        }
    }

    window.deleteUser = async function(id) {
        if (!confirm('このユーザーを削除してもよろしいですか？')) {
            return;
        }

        try {
            await apiClient.delete(id);
            await loadUsers(); // 再読み込み
        } catch (error) {
            alert('削除に失敗しました: ' + error.message);
        }
    };

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // 初回読み込み
    loadUsers();

    // 再読み込みボタン
    document.getElementById('btnReload').addEventListener('click', loadUsers);
</script>
```

### 4-2. Controllerにアクション追加

**ファイル**: `Areas/UserManagement/Controllers/UserController.cs`

```csharp
// 既存のUserControllerに追加

public IActionResult IndexApi()
{
    return View();
}
```

## Phase 5: 開発ワークフロー（日常作業）

### 5-1. TypeScript監視モード起動

```bash
# ターミナル1: TypeScriptのwatch
npm run watch:ts
```

### 5-2. ASP.NET Core起動

```bash
# ターミナル2: ASP.NET Core
dotnet watch run
```

### 5-3. ブラウザでアクセス

```
http://localhost:5000/UserManagement/User/IndexApi
```

## 🎓 学習ポイント

### 会社との共通点

✅ **APIClient.ts**: fetch APIでCRUD操作
✅ **型安全性**: TypeScriptの型定義
✅ **非同期処理**: async/await
✅ **エラーハンドリング**: try-catch
✅ **JSON通信**: application/json

### MVC版との違い

| 項目 | MVC版 | API版 |
|-----|-------|-------|
| 通信 | フォーム送信 | fetch/Ajax |
| レスポンス | HTML全体 | JSON |
| 画面更新 | ページリロード | 部分更新 |
| UX | 従来型 | モダン |

## 🚀 次のステップ

1. **ユーザー作成フォームのAjax化**
2. **属性値の動的フォーム生成**
3. **リアルタイムバリデーション**
4. **React化**（必要に応じて）

---

まずはこの形式で実装して、会社のコードと比較してみてください！
