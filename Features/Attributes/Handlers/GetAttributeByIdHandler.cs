using HelloCSharp.Features.Attributes.Queries;
using HelloCSharp.Models;
using HelloCSharp.Services;
using MediatR;

namespace HelloCSharp.Features.Attributes.Handlers;

/// <summary>
/// GetAttributeByIdQueryのハンドラー
/// </summary>
public class GetAttributeByIdHandler : IRequestHandler<GetAttributeByIdQuery, AttributeDefinition?>
{
    private readonly IAttributeService _attributeService;
    private readonly ILogger<GetAttributeByIdHandler> _logger;

    public GetAttributeByIdHandler(
        IAttributeService attributeService,
        ILogger<GetAttributeByIdHandler> logger)
    {
        _attributeService = attributeService;
        _logger = logger;
    }

    public async Task<AttributeDefinition?> Handle(
        GetAttributeByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("属性定義を取得します: ID={Id}", request.Id);
        var attribute = await _attributeService.GetByIdAsync(request.Id);
        
        if (attribute == null)
        {
            _logger.LogWarning("属性定義が見つかりませんでした: ID={Id}", request.Id);
        }
        
        return attribute;
    }
}
