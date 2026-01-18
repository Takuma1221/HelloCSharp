using MediatR;

namespace HelloCSharp.Features.Attributes.Commands;

/// <summary>
/// 属性定義を削除するコマンド
/// </summary>
public record DeleteAttributeCommand(int Id) : IRequest<bool>;
