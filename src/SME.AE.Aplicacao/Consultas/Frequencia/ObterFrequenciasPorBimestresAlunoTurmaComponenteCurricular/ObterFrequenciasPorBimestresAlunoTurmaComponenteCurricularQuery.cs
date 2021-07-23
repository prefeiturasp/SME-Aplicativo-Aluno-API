using FluentValidation;
using MediatR;
using SME.AE.Aplicacao.Comum;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery : IRequest<IEnumerable<FrequenciaAlunoDto>>
    {
        public int[] Bimestres { get; set; }
        public string TurmaCodigo { get; set; }
        public string AlunoCodigo { get; set; }
        public string ComponenteCurricularId { get; set; }

        public ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery(int[] bimestres, string turmaCodigo, string alunoCodigo, string componenteCurricularId)
        {
            Bimestres = bimestres;
            TurmaCodigo = turmaCodigo;
            AlunoCodigo = alunoCodigo;
            ComponenteCurricularId = componenteCurricularId;
        }
    }

    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryValidator : AbstractValidator<ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery>
    {
        public ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryValidator()
        {
            RuleFor(a => a.Bimestres)
                .NotNull()
                .NotEmpty()
                .WithMessage("Necessário informar os bimestres");
            RuleFor(a => a.TurmaCodigo)
                .NotNull()
                .NotEmpty()
                .WithMessage("Necessário informar o id da turma");
            RuleFor(a => a.ComponenteCurricularId)
               .NotNull()
               .NotEmpty()
               .WithMessage("Necessário informar o id do componente curricular");
            RuleFor(a => a.AlunoCodigo)
               .NotNull()
               .NotEmpty()
               .WithMessage("Necessário informar o código do aluno");
        }
    }
}
