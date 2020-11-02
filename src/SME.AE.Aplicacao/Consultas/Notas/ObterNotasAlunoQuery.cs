using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQuery : IRequest<IEnumerable<NotaAlunoResposta>>
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }

        public ObterNotasAlunoQuery(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno)
        {
            AnoLetivo = anoLetivo;
            CodigoUe = codigoUe;
            CodigoTurma = codigoTurma;
            CodigoAluno = codigoAluno;
        }
    }
}
