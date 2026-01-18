using HelloCSharp.Features.Attributes.Commands;
using HelloCSharp.Models;
using HelloCSharp.Services;
using MediatR;

namespace HelloCSharp.Features.Attributes.Handlers;

/// <summary>
/// CreateAttributeCommandのハンドラー
/// </summary>
public class CreateAttributeHandler : IRequestHandler<CreateAttributeCommand, AttributeDefinition>
{
    private readonly IAttributeService _attributeService;
    private readonly ILogger<CreateAttributeHandler> _logger;

    public CreateAttributeHandler(
        IAttributeService attributeService,
        ILogger<CreateAttributeHandler> logger)
    {
        _attributeService = attributeService;
        _logger = logger;
    }

    public async Task<AttributeDefinition> Handle(
        CreateAttributeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("属性定義を作成します: {AttributeName}", request.AttributeName);

        var attribute = new AttributeDefinition
        {
            AttributeName = request.AttributeName,
            DataType = request.DataType,
            DisplayOrder = request.DisplayOrder,
            IsRequired = request.IsRequired,
            CreatedAt = DateTime.Now
        };

        var created = await _attributeService.CreateAsync(attribute);
        _logger.LogInformation("属性定義を作成しました: ID={Id}, Name={Name}", created.Id, created.AttributeName);
        
        return created;
    }
}
