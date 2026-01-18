using MediatR;
using FluentValidation;

namespace HelloCSharp.Infrastructure.Behaviors;

/// <summary>
/// MediatRリクエストの例外ハンドリングを行うBehavior
/// すべての例外をキャッチして適切にログ出力し、再スロー
/// </summary>
public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            // FluentValidationの例外は再スロー（Controller側でハンドリング）
            _logger.LogWarning("バリデーションエラー: {Errors}", 
                string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(ex, "【権限エラー】リクエスト: {RequestName} で権限エラーが発生しました", requestName);
            throw;
        }
        catch (KeyNotFoundException ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(ex, "【NotFound】リクエスト: {RequestName} でリソースが見つかりませんでした", requestName);
            throw;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, 
                "【予期しないエラー】リクエスト: {RequestName} で予期しないエラーが発生しました。{@Request}", 
                requestName, 
                request);
            throw;
        }
    }
}
