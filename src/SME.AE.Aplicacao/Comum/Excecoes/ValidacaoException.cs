using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.Practices.ObjectBuilder2;

namespace SME.AE.Aplicacao.Comum.Excecoes
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
            errors.ForEach(x => Errors.Add(x.Key, x.Value));
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}