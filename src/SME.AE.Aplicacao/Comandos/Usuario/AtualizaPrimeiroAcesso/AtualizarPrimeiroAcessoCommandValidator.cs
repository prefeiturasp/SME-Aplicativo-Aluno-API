using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class AtualizarPrimeiroAcessoCommandValidator : AbstractValidator<AtualizarPrimeiroAcessoCommand>
    {
        public AtualizarPrimeiroAcessoCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O Id deve ser informado e Maior que 0");
        }
    }
}
