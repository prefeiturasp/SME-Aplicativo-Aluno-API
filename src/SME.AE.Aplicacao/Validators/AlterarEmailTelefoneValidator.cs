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
            RuleFor(x => x.Id).NotNull().WithMessage("O Id deve ser informado").GreaterThan(0).WithMessage("O Id deve ser informado");
            
            RuleFor(x => x.Celular).NotEmpty().WithMessage("O Celular deve ser informado").Matches(@"(\(\d{2}\)\s)(\d{4,5}\-\d{4})").WithMessage("O Celular esta fora do padrão (00) 00000-0000").When(x => string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Celular));
            
            RuleFor(x => x.Email).NotEmpty().WithMessage("O Email deve ser informado").EmailAddress().WithMessage("O Email está em formato invalido").When(x => string.IsNullOrWhiteSpace(x.Celular) || !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
