using FluentValidation;
using SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular;
using SME.AE.Aplicacao.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular
{
    public class AlterarEmailCelularCommandValidator : AbstractValidator<AlterarEmailCelularCommand>
    {
        public AlterarEmailCelularCommandValidator()
        {
            RuleFor(x => x.AlterarEmailCelularDto).NotNull().SetValidator(new AlterarEmailTelefoneValidator()).WithMessage($"O Objeto AlterarEmailCelularDto é Obrigátorio");            
        }
    }
}
