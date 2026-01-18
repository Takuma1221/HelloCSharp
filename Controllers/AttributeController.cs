using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Controllers;

/// <summary>
/// 属性管理のViewコントローラー
/// React版のHTMLを返すだけ（実際のCRUDはAttributeSqlControllerが担当）
/// 
/// 対応関係:
/// - View: Views/Attribute/Index.cshtml
/// - React: Scripts/react/pages/AttributePage.tsx（Phase 3強化版）
/// - API: Controllers/Api/AttributeSqlController.cs
/// 
/// Phase 3強化内容:
/// - React Query: サーバーステート管理（キャッシュ、自動再取得）
/// - Jotai: クライアントステート管理（Atoms）
/// - TanStack Table: ソート、フィルタ、ページング
/// </summary>
public class AttributeController : Controller
{
    // GET: /Attribute
    // Phase 3強化版 属性管理画面を返す
    public IActionResult Index()
    {
        return View();
    }
}
