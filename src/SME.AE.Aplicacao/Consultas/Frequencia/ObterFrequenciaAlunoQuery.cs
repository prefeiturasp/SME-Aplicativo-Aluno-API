using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.Frequencia
{
    public class ObterFrequenciaAlunoQuery : IRequest<IEnumerable<FrequenciaAlunoResposta>>
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }

        public ObterFrequenciaAlunoQuery(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno)
        {
            AnoLetivo = anoLetivo;
            CodigoUe = codigoUe;
            CodigoTurma = codigoTurma;
            CodigoAluno = codigoAluno;
        }
    }
}
