using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Exemplo.ObterExemplo
{
    public class ObterExemploUseCaseValidator : AbstractValidator<ObterExemploCommand>
    {
        public ObterExemploUseCaseValidator()
        {
            RuleFor(v => v).NotNull();
        }
    }
}
