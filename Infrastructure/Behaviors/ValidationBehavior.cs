using FluentValidation;
using MediatR;

namespace HelloCSharp.Infrastructure.Behaviors;

/// <summary>
/// MediatRリクエストのバリデーションを行うBehavior
/// FluentValidationを使って、Commandの実行前にバリデーションを実行
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(
                "【バリデーションエラー】リクエスト: {RequestName} で {ErrorCount} 件のエラーが発生しました",
                requestName,
                failures.Count);

            throw new ValidationException(failures);
        }

        return await next();
    }
}
