using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

/// <summary>
/// ユーザー管理のWeb API Controller（サービス層使用版）
/// ビジネスロジックはUserServiceに委譲
/// </summary>
[Area("UserManagement")]
[ApiController]
[Route("api/[area]/[controller]")]
public class UserSqlController : ControllerBase
{
    private readonly IUserService _userService;

    public UserSqlController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// ユーザー一覧を取得
    /// GET: /api/UserManagement/UserSql
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>
    /// 特定のユーザーを取得
    /// GET: /api/UserManagement/UserSql/5
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return NotFound(new { message = "ユーザーが見つかりません", id });
        }

        return Ok(user);
    }

    /// <summary>
    /// 新規ユーザーを作成
    /// POST: /api/UserManagement/UserSql
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // メールアドレスの重複チェック
        if (await _userService.EmailExistsAsync(user.Email))
        {
            return BadRequest(new { message = "このメールアドレスは既に使用されています" });
        }

        var created = await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// ユーザーを更新
    /// PUT: /api/UserManagement/UserSql/5
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest(new { message = "IDが一致しません" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // メールアドレスの重複チェック（自分以外）
        if (await _userService.EmailExistsAsync(user.Email, id))
        {
            return BadRequest(new { message = "このメールアドレスは既に使用されています" });
        }

        var success = await _userService.UpdateAsync(user);
        
        if (!success)
        {
            return NotFound(new { message = "ユーザーが見つかりません", id });
        }

        return Ok(user);
    }

    /// <summary>
    /// ユーザーを削除
    /// DELETE: /api/UserManagement/UserSql/5
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteAsync(id);
        
        if (!success)
        {
            return NotFound(new { message = "ユーザーが見つかりません", id });
        }

        return NoContent();
    }
}
