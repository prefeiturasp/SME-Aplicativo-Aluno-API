using FluentValidation;
using SME.AE.Aplicacao.Comandos.TermosDeUso;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    class RegistrarAceiteDosTermosDeUsoCommandValidatior : AbstractValidator<RegistrarAceiteDosTermosDeUsoCommand>
    {
        public RegistrarAceiteDosTermosDeUsoCommandValidatior()
        {
            RuleFor(v => v.Usuario).NotNull().WithMessage("O campo Usuário é obrigatório");
            RuleFor(v => v.Ip).NotNull().WithMessage("O campo IP é obrigatório");
            RuleFor(v => v.TermoDeUsoId).NotNull().WithMessage("O campo ID do Termo dé uso é obrigatório");
            RuleFor(v => v.Ip).MinimumLength(8).MaximumLength(12).WithMessage("O campo IP deve ter entre 8 e 12 caracteres");
            RuleFor(v => v.Device).NotNull().WithMessage("O campo Device é obrigatório");
            RuleFor(v => v.Versao).NotNull().WithMessage("O campo Versão é obrigatório");
        }
    }
}

