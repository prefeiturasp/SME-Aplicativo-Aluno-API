using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoCorPorParametroQueryValidator : AbstractValidator<ObterFrequenciaAlunoCorPorParametroQuery>
    {
        public ObterFrequenciaAlunoCorPorParametroQueryValidator()
        {
            RuleFor(x => x.Parametros).NotEmpty().WithMessage("Os parâmetros devem ser informados");
        }
    }
}
