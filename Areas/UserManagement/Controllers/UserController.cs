using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.UserManagement.Controllers;

/// <summary>
/// ユーザー管理のViewコントローラー
/// React版のHTMLを返すだけ（実際のCRUDはUserSqlControllerが担当）
/// 
/// 対応関係:
/// - View: Views/User/Index.cshtml
/// - React: Scripts/react/pages/UserPage.tsx
/// - API: Controllers/Api/UserSqlController.cs
/// </summary>
[Area("UserManagement")]
public class UserController : Controller
{
    // GET: /UserManagement/User
    // React版のユーザー管理画面を返す
    public IActionResult Index()
    {
        return View();
    }
}
