using FluentValidation;
using SME.AE.Aplicacao.Comandos.Usuario.ReiniciarSenha;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class ReiniciarSenhaCommandValidator : AbstractValidator<ReiniciarSenhaCommand>
    {
        public ReiniciarSenhaCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("O Id deve ser informado e Maior que 0");
        }
    }
}
