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
            RuleFor(x => x.UsuarioIdCore).NotNull();
            RuleFor(x => x.SenhaCriptografada).NotNull();
        }
    }
}
