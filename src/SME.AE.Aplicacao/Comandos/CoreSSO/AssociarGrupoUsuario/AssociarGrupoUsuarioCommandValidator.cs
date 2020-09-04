using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario
{
    public class AssociarGrupoUsuarioCommandValidator : AbstractValidator<AssociarGrupoUsuarioCommand>
    {
        public AssociarGrupoUsuarioCommandValidator()
        {
            RuleFor(x => x.UsuarioCoreSSO).NotNull().WithMessage("È obrigatório informar um usuário para incluir os grupos");
        }
    }
}
