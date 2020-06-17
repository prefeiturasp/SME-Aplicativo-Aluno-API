using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class AlterarEmailTelefoneValidator : AbstractValidator<AlterarEmailCelularDto>
    {
        public AlterarEmailTelefoneValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0);
            RuleFor(x => x.Celular).NotNull().NotEmpty().Matches(@"(\(\d{2}\)\s)(\d{4,5}\-\d{4})").When(x => string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Celular));
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().When(x => string.IsNullOrWhiteSpace(x.Celular) || !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
