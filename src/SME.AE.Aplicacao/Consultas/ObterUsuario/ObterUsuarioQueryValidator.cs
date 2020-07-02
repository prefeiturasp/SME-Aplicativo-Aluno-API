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
            RuleFor(x => x.Cpf).NotEmpty().When(x => x.Id == 0 || !string.IsNullOrWhiteSpace(x.Cpf)).WithMessage("O CPF é Obrigátorio");
            RuleFor(x => x.Id).GreaterThan(0).When(x => string.IsNullOrWhiteSpace(x.Cpf) || x.Id != 0).WithMessage("O Id é Obrigátorio");
        }
    }
}
