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
/// </summary>
public class AttributeController : Controller
{
    // GET: /UserManagement/Attribute
    // React版の属性管理画面を返す
    public IActionResult Index()
    {
        return View();
    }
}
