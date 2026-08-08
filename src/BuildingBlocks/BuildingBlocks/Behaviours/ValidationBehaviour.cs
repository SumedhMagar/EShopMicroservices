
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviours
{
    public class ValidationBehaviour<TRequest,TResponse>(IEnumerable<IValidator<TRequest>> validators):IPipelineBehavior<TRequest,TResponse> 
        where TRequest : CQRS.ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Perform validation logic here
            // For example, you can use FluentValidation or any other validation library
            // If validation fails, you can throw an exception or return an error response
            // If validation passes, call the next delegate in the pipeline
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v=>v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.SelectMany(r=>r.Errors).Where(f=>f!=null).ToList();

            if(failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
