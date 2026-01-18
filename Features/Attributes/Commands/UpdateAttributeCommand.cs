using HelloCSharp.Models;
using MediatR;

namespace HelloCSharp.Features.Attributes.Commands;

/// <summary>
/// 属性定義を更新するコマンド
/// </summary>
public record UpdateAttributeCommand(
    int Id,
    string AttributeName,
    string DataType,
    int DisplayOrder,
    bool IsRequired
) : IRequest<AttributeDefinition?>;
