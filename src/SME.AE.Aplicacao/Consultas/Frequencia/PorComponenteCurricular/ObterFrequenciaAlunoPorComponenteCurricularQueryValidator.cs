using FluentValidation;
using SME.AE.Aplicacao.Consultas.Frequencia;

namespace SME.AE.Aplicacao.Consultas.Frequencia.PorComponenteCurricular
{
    public class ObterFrequenciaAlunoPorComponenteCurricularQueryValidator : AbstractValidator<ObterFrequenciaAlunoPorComponenteCurricularQuery>
    {
        public ObterFrequenciaAlunoPorComponenteCurricularQueryValidator()
        {
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O código da Unidade Escolar (UE) é obrigátorio");
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma é obrigátorio");
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("O código do Aluno é obrigátorio");

            RuleFor(x => x.AnoLetivo).NotEmpty().WithMessage("Ano letivo é obrigatório");
            RuleFor(x => x.AnoLetivo).InclusiveBetween(2020, 9999).WithMessage("Ano letivo deve estar entre 2020 e 9999");

            RuleFor(x => x.CodigoComponenteCurricular).NotEmpty().WithMessage("O código do componente curricular é obrigátorio.");
        }
    }
}
