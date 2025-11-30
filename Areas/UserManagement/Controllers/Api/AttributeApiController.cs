using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloCSharp.Areas.UserManagement.Controllers.Api;

/// <summary>
/// 属性管理のWeb API Controller
/// RESTful APIとしてJSON形式でデータをやり取りする
/// </summary>
[Area("UserManagement")]
[ApiController]
[Route("api/[area]/[controller]")]  // => /api/UserManagement/Attribute
public class AttributeApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttributeApiController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 属性一覧を取得
    /// GET: /api/UserManagement/Attribute
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttributeDefinition>>> GetAll()
    {
        var attributes = await _context.Attributes
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();

        return Ok(attributes);
    }

    /// <summary>
    /// 特定の属性を取得
    /// GET: /api/UserManagement/Attribute/5
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AttributeDefinition>> GetById(int id)
    {
        var attribute = await _context.Attributes.FindAsync(id);

        if (attribute == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(attribute);
    }

    /// <summary>
    /// 新規属性を作成
    /// POST: /api/UserManagement/Attribute
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AttributeDefinition>> Create([FromBody] AttributeDefinition attribute)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Attributes.Add(attribute);
        await _context.SaveChangesAsync();

        // 201 Created を返し、Location ヘッダーに作成されたリソースのURLを設定
        return CreatedAtAction(
            nameof(GetById),
            new { id = attribute.Id },
            attribute
        );
    }

    /// <summary>
    /// 属性を更新
    /// PUT: /api/UserManagement/Attribute/5
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

        // 既存のエンティティを取得して更新
        var existing = await _context.Attributes.FindAsync(id);
        if (existing == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        // 更新するプロパティを設定
        existing.AttributeName = attribute.AttributeName;
        existing.DataType = attribute.DataType;
        existing.DisplayOrder = attribute.DisplayOrder;
        existing.IsRequired = attribute.IsRequired;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AttributeExists(id))
            {
                return NotFound(new { message = "属性が見つかりません", id });
            }
            throw;
        }

        return Ok(existing);
    }

    /// <summary>
    /// 属性を削除
    /// DELETE: /api/UserManagement/Attribute/5
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var attribute = await _context.Attributes
            .Include(a => a.UserAttributeValues)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attribute == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        // 使用中の属性は削除できない（オプション：警告を返す）
        if (attribute.UserAttributeValues.Any())
        {
            return BadRequest(new
            {
                message = "この属性は使用中のため削除できません",
                usageCount = attribute.UserAttributeValues.Count
            });
        }

        _context.Attributes.Remove(attribute);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }

    private async Task<bool> AttributeExists(int id)
    {
        return await _context.Attributes.AnyAsync(e => e.Id == id);
    }
}
