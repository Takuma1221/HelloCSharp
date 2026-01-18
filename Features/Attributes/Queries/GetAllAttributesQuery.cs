using HelloCSharp.Models;
using MediatR;

namespace HelloCSharp.Features.Attributes.Queries;

/// <summary>
/// すべての属性定義を取得するクエリ
/// </summary>
public record GetAllAttributesQuery : IRequest<IEnumerable<AttributeDefinition>>;
