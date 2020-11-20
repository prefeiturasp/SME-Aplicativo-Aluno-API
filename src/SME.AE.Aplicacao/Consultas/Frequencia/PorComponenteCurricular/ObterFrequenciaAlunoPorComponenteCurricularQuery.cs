using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;

namespace SME.AE.Aplicacao.Consultas.Frequencia.PorComponenteCurricular
{
    public class ObterFrequenciaAlunoPorComponenteCurricularQuery : IRequest<FrequenciaAlunoPorComponenteCurricularResposta>
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public short CodigoComponenteCurricular { get; set; }

        public ObterFrequenciaAlunoPorComponenteCurricularQuery(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno, short codigoComponenteCurricular)
        {
            AnoLetivo = anoLetivo;
            CodigoUe = codigoUe;
            CodigoTurma = codigoTurma;
            CodigoAluno = codigoAluno;
            CodigoComponenteCurricular = codigoComponenteCurricular;
        }
    }
}