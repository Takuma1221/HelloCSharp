using MediatR;

namespace HelloCSharp.Infrastructure.Behaviors;

/// <summary>
/// MediatRリクエストのロギングを行うBehavior
/// すべてのCommand/Queryの実行前後にログを出力
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("【開始】リクエスト: {RequestName} {@Request}", requestName, request);

        try
        {
            var response = await next();
            
            _logger.LogInformation("【完了】リクエスト: {RequestName}", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "【エラー】リクエスト: {RequestName} でエラーが発生しました", requestName);
            throw;
        }
    }
}
