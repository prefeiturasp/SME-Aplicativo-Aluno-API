using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Validators
{
    public class UsuarioCoreSSOValidator : AbstractValidator<UsuarioCoreSSODto>
    {
        public UsuarioCoreSSOValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF deve ser informado").ValidarCpf();
            RuleFor(x => x.Nome).NotEmpty().WithMessage("O Nome deve ser informado");
            RuleFor(x => x.Senha).NotEmpty().WithMessage("A Senha deve ser informada");
        }
    }
}
