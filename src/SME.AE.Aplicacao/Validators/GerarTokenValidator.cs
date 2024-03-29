﻿using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;

namespace SME.AE.Aplicacao.Validators
{
    public class GerarTokenValidator : AbstractValidator<GerarTokenDto>
    {
        public GerarTokenValidator()
        {
            RuleFor(x => x.CPF).NotEmpty().WithMessage("È necessário informar o CPF");

            RuleFor(x => x.CPF).ValidarCpf().When(x => !string.IsNullOrWhiteSpace(x.CPF));
        }
    }
}
