using HelloCSharp.Features.Attributes.Queries;
using HelloCSharp.Models;
using HelloCSharp.Services;
using MediatR;

namespace HelloCSharp.Features.Attributes.Handlers;

/// <summary>
/// GetAllAttributesQueryのハンドラー
/// </summary>
public class GetAllAttributesHandler : IRequestHandler<GetAllAttributesQuery, IEnumerable<AttributeDefinition>>
{
    private readonly IAttributeService _attributeService;
    private readonly ILogger<GetAllAttributesHandler> _logger;

    public GetAllAttributesHandler(
        IAttributeService attributeService,
        ILogger<GetAllAttributesHandler> logger)
    {
        _attributeService = attributeService;
        _logger = logger;
    }

    public async Task<IEnumerable<AttributeDefinition>> Handle(
        GetAllAttributesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("すべての属性定義を取得します");
        var attributes = await _attributeService.GetAllAsync();
        _logger.LogInformation("{Count}件の属性定義を取得しました", attributes.Count());
        return attributes;
    }
}
