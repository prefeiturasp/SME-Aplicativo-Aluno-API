using FluentValidation;
using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterAlunosPorTurmaQuery : IRequest<IEnumerable<AlunoTurmaEol>>
    {

        public ObterAlunosPorTurmaQuery(long codigoTurma)
        {
            CodigoTurma = codigoTurma;
        }

        public long CodigoTurma { get; set; }
    }

    public class ObterAlunosPorTurmaQueryValidator : AbstractValidator<ObterAlunosPorTurmaQuery>
    {
        public ObterAlunosPorTurmaQueryValidator()
        {
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma é Obrigátorio");
        }
    }
}
