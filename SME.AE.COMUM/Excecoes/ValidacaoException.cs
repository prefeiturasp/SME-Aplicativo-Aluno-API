using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SME.AE.Comum.Excecoes
{
    public class ValidacaoException : Exception
    {
        public ValidacaoException()
            : base("Ocorreu um ou mais erros de validação.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidacaoException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public ValidacaoException(IDictionary<string, string[]> errors)
        {
            Errors = errors;
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}