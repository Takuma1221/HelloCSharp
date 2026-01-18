using HelloCSharp.Models;
using MediatR;

namespace HelloCSharp.Features.Attributes.Queries;

/// <summary>
/// IDで属性定義を取得するクエリ
/// </summary>
public record GetAttributeByIdQuery(int Id) : IRequest<AttributeDefinition?>;
