using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WebApplication.Core.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace WebApplication.Core.Common.Behaviours
{
    public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        
        /// <inheritdoc />
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (!failures.Any()) return await next();

            ValidationFailure? notFoundFailure = failures.Find(x => x.PropertyName.Equals("Not Found"));
            
            if (notFoundFailure != default) throw new NotFoundException(notFoundFailure.ErrorMessage);
            throw new ValidationException(failures);
        }
    }
}
