using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Message"] = "ASP.NET Core MVCへようこそ";
        return View();
    }

    public IActionResult About()
    {
        ViewData["Message"] = "このアプリは学習用の最小MVCサンプルです";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
