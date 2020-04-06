using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Exemplo.ObterExemplo
{
    public class ObterExemploCommandValidator : AbstractValidator<ObterExemploCommand>
    {
        public ObterExemploCommandValidator()
        {
            RuleFor(v => v).NotNull();
        }
    }
}
