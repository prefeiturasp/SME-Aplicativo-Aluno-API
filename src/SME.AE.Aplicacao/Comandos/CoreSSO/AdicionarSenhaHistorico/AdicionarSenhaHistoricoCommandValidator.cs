using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico
{
    public class AdicionarSenhaHistoricoCommandValidator : AbstractValidator<AdicionarSenhaHistoricoCommand>
    {
        public AdicionarSenhaHistoricoCommandValidator()
        {
            RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("Deve ser informado o ID do usuário");
            RuleFor(x => x.SenhaCriptografada).NotEmpty().WithMessage("Deve ser informada a senha Criptografada");
        }
    }
}
