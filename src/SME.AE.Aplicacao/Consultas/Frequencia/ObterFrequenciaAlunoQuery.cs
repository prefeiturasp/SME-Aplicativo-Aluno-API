using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.Frequencia
{
    public class ObterFrequenciaAlunoQuery : IRequest<IEnumerable<FrequenciaAlunoResposta>>
    {
        public string CodigoUe { get; set; }
        public long CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }

        public ObterFrequenciaAlunoQuery(string codigoUe, long codigoTurma, string codigoAluno)
        {
            CodigoUe = codigoUe;
            CodigoTurma = codigoTurma;
            CodigoAluno = codigoAluno;
        }
    }
}
