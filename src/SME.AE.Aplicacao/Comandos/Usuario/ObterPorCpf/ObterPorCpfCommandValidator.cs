using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf
{
    public class ObterPorCpfCommandValidator : AbstractValidator<ObterUsuarioPorCpfCommand>
    {
        public ObterPorCpfCommandValidator()
        {
            RuleFor(v => v).NotNull();
            RuleFor(v => v.Cpf).NotNull();
        }
    }
}