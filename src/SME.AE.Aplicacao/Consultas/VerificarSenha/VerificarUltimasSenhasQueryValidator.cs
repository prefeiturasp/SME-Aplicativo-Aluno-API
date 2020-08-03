using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.VerificarSenha
{
    public class VerificarUltimasSenhasQueryValidator : AbstractValidator<VerificarUltimasSenhasQuery>
    {
        public VerificarUltimasSenhasQueryValidator()
        {
            RuleFor(x => x.UsuarioIdCore).NotEmpty().WithMessage("O Id do Usuário no CoreSSO é Obrigátorio"); ;
            RuleFor(x => x.SenhaCriptografada).NotEmpty().WithMessage("O Id é Obrigátorio"); ;
        }
    }
}
