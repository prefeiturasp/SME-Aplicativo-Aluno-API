using FluentValidation;
using SME.AE.Aplicacao.Consultas.Frequencia;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterFrequenciaAlunoQueryValidator : AbstractValidator<ObterFrequenciaAlunoQuery>
    {
        public ObterFrequenciaAlunoQueryValidator()
        {
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O código da Unidade Escolar (UE) é obrigátorio");
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma é obrigátorio");
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("O código do Aluno é obrigátorio");
        }
    }
}
