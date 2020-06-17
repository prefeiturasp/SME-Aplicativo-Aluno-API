using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.Usuario
{
    public class CriarUsuarioCoreSSOCommandValidator : AbstractValidator<CriarUsuarioCoreSSOCommand>
    {
        public CriarUsuarioCoreSSOCommandValidator()
        {
            RuleFor(x => x.Usuario).NotNull();            
        }
    }
}
