using HelloCSharp.Models;
using MediatR;

namespace HelloCSharp.Features.Attributes.Commands;

/// <summary>
/// 新しい属性定義を作成するコマンド
/// </summary>
public record CreateAttributeCommand(
    string AttributeName,
    string DataType,
    int DisplayOrder,
    bool IsRequired = false
) : IRequest<AttributeDefinition>;
