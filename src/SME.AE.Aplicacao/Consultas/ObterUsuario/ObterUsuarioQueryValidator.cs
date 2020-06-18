using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioQueryValidator : AbstractValidator<ObterUsuarioQuery>
    {
        public ObterUsuarioQueryValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().When(x => x.Id == 0);
            RuleFor(x => x.Id).GreaterThan(0).NotNull().When(x => string.IsNullOrWhiteSpace(x.Cpf));
        }
    }
}
