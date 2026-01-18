using HelloCSharp.Features.Attributes.Commands;
using HelloCSharp.Services;
using MediatR;

namespace HelloCSharp.Features.Attributes.Handlers;

/// <summary>
/// DeleteAttributeCommandのハンドラー
/// </summary>
public class DeleteAttributeHandler : IRequestHandler<DeleteAttributeCommand, bool>
{
    private readonly IAttributeService _attributeService;
    private readonly ILogger<DeleteAttributeHandler> _logger;

    public DeleteAttributeHandler(
        IAttributeService attributeService,
        ILogger<DeleteAttributeHandler> logger)
    {
        _attributeService = attributeService;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeleteAttributeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("属性定義を削除します: ID={Id}", request.Id);

        var existing = await _attributeService.GetByIdAsync(request.Id);
        if (existing == null)
        {
            _logger.LogWarning("削除対象の属性定義が見つかりませんでした: ID={Id}", request.Id);
            return false;
        }

        await _attributeService.DeleteAsync(request.Id);
        _logger.LogInformation("属性定義を削除しました: ID={Id}", request.Id);
        
        return true;
    }
}
