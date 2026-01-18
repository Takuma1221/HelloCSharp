using HelloCSharp.Models;
using HelloCSharp.Features.Attributes.Commands;
using HelloCSharp.Features.Attributes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Controllers.Api;

/// <summary>
/// 属性管理のWeb API Controller（CQRS + MediatR版）
/// ビジネスロジックはHandlerに委譲
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AttributeSqlController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttributeSqlController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 属性一覧を取得
    /// GET: /api/AttributeSql
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttributeDefinition>>> GetAll()
    {
        var query = new GetAllAttributesQuery();
        var attributes = await _mediator.Send(query);
        return Ok(attributes);
    }

    /// <summary>
    /// 特定の属性を取得
    /// GET: /api/AttributeSql/5
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AttributeDefinition>> GetById(int id)
    {
        var query = new GetAttributeByIdQuery(id);
        var attribute = await _mediator.Send(query);
        
        if (attribute == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(attribute);
    }

    /// <summary>
    /// 新規属性を作成
    /// POST: /api/AttributeSql
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AttributeDefinition>> Create([FromBody] CreateAttributeCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// 属性を更新
    /// PUT: /api/AttributeSql/5
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttributeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "IDが一致しません" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _mediator.Send(command);
        
        if (updated == null)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return Ok(updated);
    }

    /// <summary>
    /// 属性を削除
    /// DELETE: /api/AttributeSql/5
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteAttributeCommand(id);
        var success = await _mediator.Send(command);
        
        if (!success)
        {
            return NotFound(new { message = "属性が見つかりません", id });
        }

        return NoContent();
    }
}
