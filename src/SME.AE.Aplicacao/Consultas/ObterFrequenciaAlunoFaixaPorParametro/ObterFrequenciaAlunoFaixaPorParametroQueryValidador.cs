using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoFaixaPorParametroQueryValidator : AbstractValidator<ObterFrequenciaAlunoFaixaPorParametroQuery>
    {
        public ObterFrequenciaAlunoFaixaPorParametroQueryValidator()
        {
            RuleFor(x => x.Parametros).NotEmpty().WithMessage("Os parâmetros devem ser informados");
        }
    }
}
