using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class UsuarioNotificacaoValidator : AbstractValidator<UsuarioNotificacaoDto>
    {
        public UsuarioNotificacaoValidator()
        {
           // RuleFor(x => x.CpfUsuario).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Deve ser informado o CPF");
           // RuleFor(x => x.CpfUsuario).ValidarCpf().WithMessage("CPF com Formato Invalido").When(x => !string.IsNullOrWhiteSpace(x.CpfUsuario));
            RuleFor(x => x.NotificacaoId).NotEmpty().WithMessage("Deve ser informado o Id da Notificação");
        }
    }
}
