using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Middlewares
{
    public class ValidacaoRequisicaoMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidacaoRequisicaoMiddleware(IEnumerable<IValidator<TRequest>> validators)
        {
            this._validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return next();

            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures?.Any() ?? false)
                throw new ValidacaoException(failures);


            return next();
        }
    }
}