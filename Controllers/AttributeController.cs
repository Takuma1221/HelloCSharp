using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Controllers;

/// <summary>
/// 属性管理のViewコントローラー
/// React版のHTMLを返すだけ（実際のCRUDはAttributeSqlControllerが担当）
/// 
/// 対応関係:
/// - View: Views/Attribute/Index.cshtml
/// - React: Scripts/react/pages/AttributePage.tsx
/// - API: Controllers/Api/AttributeSqlController.cs
/// 
/// Phase 3版（V2）:
/// - View: Views/Attribute/IndexV2.cshtml
/// - React: Scripts/react/pages/AttributePageV2.tsx
/// - 使用技術: React Query + Jotai + TanStack Table
/// </summary>
public class AttributeController : Controller
{
    // GET: /UserManagement/Attribute
    // React版の属性管理画面を返す
    public IActionResult Index()
    {
        return View();
    }

    // GET: /UserManagement/Attribute/IndexV2
    // Phase 3強化版（React Query + Jotai + TanStack Table）
    public IActionResult IndexV2()
    {
        return View();
    }
}
