using MediatR;
using System.Diagnostics;

namespace HelloCSharp.Infrastructure.Behaviors;

/// <summary>
/// MediatRリクエストのパフォーマンス計測を行うBehavior
/// 実行時間が閾値を超えた場合に警告ログを出力
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        var requestName = typeof(TRequest).Name;

        if (elapsedMilliseconds > 500) // 500ms以上で警告
        {
            _logger.LogWarning(
                "【低速】リクエスト: {RequestName} の実行に {ElapsedMilliseconds}ms かかりました。 {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }
        else
        {
            _logger.LogInformation(
                "【パフォーマンス】リクエスト: {RequestName} 実行時間: {ElapsedMilliseconds}ms",
                requestName,
                elapsedMilliseconds);
        }

        return response;
    }
}
