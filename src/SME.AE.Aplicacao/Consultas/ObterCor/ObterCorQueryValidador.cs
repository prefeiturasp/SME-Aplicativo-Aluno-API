using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterCorQueryValidador : AbstractValidator<ObterCorQuery>
    {
        public ObterCorQueryValidador()
        {
            RuleFor(x => x.Parametros).NotEmpty().WithMessage("Os parâmetros devem ser informados");
        }
    }
}
