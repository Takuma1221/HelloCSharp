using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

/// <summary>
/// 属性管理のWeb API Controller（サービス層使用版）
/// ビジネスロジックはAttributeServiceに委譲
/// </summary>
[Area("UserManagement")]
[ApiController]
[Route("api/[area]/[controller]")]
public class AttributeSqlController : ControllerBase
{
    private readonly IAttributeService _attributeService;

    public AttributeSqlController(IAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    /// <summary>
    /// 属性一覧を取得
    /// GET: /api/UserManagement/AttributeSql
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttributeDefinition>>> GetAll()
    {
        var attributes = await _attributeService.GetAllAsync();
        return Ok(attributes);
    }

    /// <summary>
    /// 特定の属性を取得
    /// GET: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AttributeDefinition>> GetById(int id)
    {
        var attribute = await _attributeService.GetByIdAsync(id);
        
        if (attribute == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(attribute);
    }

    /// <summary>
    /// 新規属性を作成
    /// POST: /api/UserManagement/AttributeSql
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AttributeDefinition>> Create([FromBody] AttributeDefinition attribute)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 属性名の重複チェック
        if (await _attributeService.ExistsAsync(attribute.AttributeName))
        {
            return BadRequest(new { message = "同じ属性名が既に存在します" });
        }

        var created = await _attributeService.CreateAsync(attribute);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// 属性を更新
    /// PUT: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AttributeDefinition attribute)
    {
        if (id != attribute.Id)
        {
            return BadRequest(new { message = "IDが一致しません" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 属性名の重複チェック（自分以外）
        if (await _attributeService.ExistsAsync(attribute.AttributeName, id))
        {
            return BadRequest(new { message = "同じ属性名が既に存在します" });
        }

        var success = await _attributeService.UpdateAsync(attribute);
        
        if (!success)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(attribute);
    }

    /// <summary>
    /// 属性を削除
    /// DELETE: /api/UserManagement/AttributeSql/5
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _attributeService.DeleteAsync(id);
        
        if (!success)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return NoContent();
    }
}
