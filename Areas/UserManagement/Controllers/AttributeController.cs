using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.UserManagement.Controllers;

/// <summary>
/// 属性管理のViewコントローラー
/// React版のHTMLを返すだけ（実際のCRUDはAttributeApiControllerが担当）
/// </summary>
[Area("UserManagement")]
public class AttributeController : Controller
{
    // GET: /UserManagement/Attribute
    // React版の属性管理画面を返す
    public IActionResult Index()
    {
        return View("IndexReact");
    }
}
