using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO
{
    public class ObterUsuarioCoreSSOQueryValidator : AbstractValidator<ObterUsuarioCoreSSOQuery>
    {
        public ObterUsuarioCoreSSOQueryValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().When(x => x.UsuarioId == default || !string.IsNullOrEmpty(x.Cpf));
            RuleFor(x => x.UsuarioId).NotNull().NotEmpty().When(x => string.IsNullOrEmpty(x.Cpf) || x.UsuarioId != default);
        }
    }
}
