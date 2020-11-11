using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class FrequenciaAlunoResposta
    {
        public string AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public string CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public int Bimestre { get; set; }
        public IEnumerable<FrequenciaAlunoComponenteCurricular> FrequenciasPorComponenteCurricular { get; set; }

        public FrequenciaAlunoResposta()
        {
            FrequenciasPorComponenteCurricular = new List<FrequenciaAlunoComponenteCurricular>();
        }
    }
}