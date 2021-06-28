using FluentValidation;
using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterNotasPorBimestresUeAlunoTurmaQuery : IRequest<IEnumerable<NotaConceitoBimestreComponenteDto>>
    {
        public ObterNotasPorBimestresUeAlunoTurmaQuery(int[] bimestres, long turmaId, long ueId, string alunoCodigo)
        {
            Bimestres = bimestres;
            TurmaId = turmaId;
            UeId = ueId;
            AlunoCodigo = alunoCodigo;
        }

        public int[] Bimestres { get; set; }
        public long TurmaId { get; set; }
        public long UeId { get; set; }
        public string AlunoCodigo { get; set; }
    }

    public class ObterNotasPorBimestresUeAlunoTurmaQueryValidator : AbstractValidator<ObterNotasPorBimestresUeAlunoTurmaQuery>
    {
        public ObterNotasPorBimestresUeAlunoTurmaQueryValidator()
        {
            RuleFor(a => a.Bimestres)
                .NotNull()
                .NotEmpty()
                .WithMessage("Necessário informar os bimestres");
            RuleFor(a => a.TurmaId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Necessário informar o id da turma");
            RuleFor(a => a.UeId)
               .NotNull()
               .NotEmpty()
               .WithMessage("Necessário informar o id da Ue");
            RuleFor(a => a.AlunoCodigo)
               .NotNull()
               .NotEmpty()
               .WithMessage("Necessário informar o código do aluno");
        }
    }
}
