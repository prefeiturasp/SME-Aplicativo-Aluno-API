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
            RuleFor(x => x.Cpf).NotEmpty().When(x => x.UsuarioId == default || !string.IsNullOrEmpty(x.Cpf)).WithMessage("O CPF é Obrigátorio"); ;
            RuleFor(x => x.UsuarioId).NotEmpty().When(x => string.IsNullOrEmpty(x.Cpf) || x.UsuarioId != default).WithMessage("O Id do Usuário é Obrigátorio"); ;
        }
    }
}
