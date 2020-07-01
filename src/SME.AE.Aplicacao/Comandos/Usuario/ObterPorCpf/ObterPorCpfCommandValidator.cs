using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf
{
    public class ObterPorCpfCommandValidator : AbstractValidator<ObterUsuarioPorCpfCommand>
    {
        public ObterPorCpfCommandValidator()
        {
            RuleFor(v => v.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}