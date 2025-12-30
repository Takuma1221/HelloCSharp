using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

/// <summary>
/// ユーザー属性値のWeb API Controller
/// </summary>
[Area("UserManagement")]
[ApiController]
[Route("api/[area]/[controller]")]
public class UserAttributeValueController : ControllerBase
{
    private readonly IUserAttributeValueService _service;

    public UserAttributeValueController(IUserAttributeValueService service)
    {
        _service = service;
    }

    /// <summary>
    /// 特定ユーザーの属性値一覧を取得
    /// GET: /api/UserManagement/UserAttributeValue/user/5
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<UserAttributeValue>>> GetByUserId(int userId)
    {
        var values = await _service.GetByUserIdAsync(userId);
        return Ok(values);
    }

    /// <summary>
    /// ユーザーの属性値を一括保存
    /// POST: /api/UserManagement/UserAttributeValue/user/5
    /// </summary>
    [HttpPost("user/{userId}")]
    public async Task<IActionResult> SaveUserAttributes(int userId, [FromBody] Dictionary<int, string> attributeValues)
    {
        try
        {
            await _service.SaveUserAttributesAsync(userId, attributeValues);
            return Ok(new { message = "属性値を保存しました" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
