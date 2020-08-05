using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class ValidarTokenValidator : AbstractValidator<ValidarTokenDto>
    {
        public ValidarTokenValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("É Obrigátorio informar o token");

            RuleFor(x => x.Token).Length(8).WithMessage("O Tamanho do token esta invalido").When(x => !string.IsNullOrWhiteSpace(x.Token));
        }
    }
}
