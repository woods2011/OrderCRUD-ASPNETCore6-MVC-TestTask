using FluentValidation;
using MediatR;

namespace MvcAppTest.Core.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken token)
    {
        if (!_validators.Any()) 
            return await next();
        
        var validationContext = new ValidationContext<TRequest>(request);
        
        var failures = await _validators.ToAsyncEnumerable()
            .SelectAwait(async validator => await validator.ValidateAsync(validationContext, token))
            .SelectMany(validationResult => validationResult.Errors.ToAsyncEnumerable())
            .Where(failure => failure is not null)
            .ToListAsync(token);
        
        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}