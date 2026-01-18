using HelloCSharp.Features.Attributes.Commands;
using HelloCSharp.Models;
using HelloCSharp.Services;
using MediatR;

namespace HelloCSharp.Features.Attributes.Handlers;

/// <summary>
/// UpdateAttributeCommandのハンドラー
/// </summary>
public class UpdateAttributeHandler : IRequestHandler<UpdateAttributeCommand, AttributeDefinition?>
{
    private readonly IAttributeService _attributeService;
    private readonly ILogger<UpdateAttributeHandler> _logger;

    public UpdateAttributeHandler(
        IAttributeService attributeService,
        ILogger<UpdateAttributeHandler> logger)
    {
        _attributeService = attributeService;
        _logger = logger;
    }

    public async Task<AttributeDefinition?> Handle(
        UpdateAttributeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("属性定義を更新します: ID={Id}", request.Id);

        var existing = await _attributeService.GetByIdAsync(request.Id);
        if (existing == null)
        {
            _logger.LogWarning("更新対象の属性定義が見つかりませんでした: ID={Id}", request.Id);
            return null;
        }

        existing.AttributeName = request.AttributeName;
        existing.DataType = request.DataType;
        existing.DisplayOrder = request.DisplayOrder;
        existing.IsRequired = request.IsRequired;

        await _attributeService.UpdateAsync(existing);
        _logger.LogInformation("属性定義を更新しました: ID={Id}, Name={Name}", existing.Id, existing.AttributeName);
        
        return existing;
    }
}
