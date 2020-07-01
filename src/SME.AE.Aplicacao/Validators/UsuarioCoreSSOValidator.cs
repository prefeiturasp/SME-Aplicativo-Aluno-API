using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Validators
{
    public class UsuarioCoreSSOValidator : AbstractValidator<UsuarioCoreSSODto>
    {
        public UsuarioCoreSSOValidator()
        {
            RuleFor(x => x.Cpf).ValidarCpf().WithMessage("O CPF deve ser informado");
            RuleFor(x => x.Nome).NotNull().NotEmpty().WithMessage("O Nome deve ser informado");
            RuleFor(x => x.Senha).NotNull().NotEmpty().WithMessage("A Senha deve ser informada");
        }
    }
}
