using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    class AutenticarUsuarioUseCaseValidatior : AbstractValidator<AutenticarUsuarioCommand>
    {
        public AutenticarUsuarioUseCaseValidatior()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).Length(11).WithMessage("O campo CPF deve ter 11 caracteres");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido");
            RuleFor(v => v.Senha).MinimumLength(1).WithMessage("O Campo Senha é obrigatório");
        }
    }
}
