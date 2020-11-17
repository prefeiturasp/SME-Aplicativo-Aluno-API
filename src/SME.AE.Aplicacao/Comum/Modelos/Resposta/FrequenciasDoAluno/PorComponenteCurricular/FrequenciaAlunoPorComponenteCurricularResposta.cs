using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;
using System.Linq;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular
{
    public class FrequenciaAlunoPorComponenteCurricularResposta
    {
        public string AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public string CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public short CodigoComponenteCurricular { get; set; }
        public string ComponenteCurricular { get; set; }
        public ICollection<FrequenciaAlunoPorBimestre> FrequenciasPorBimestre { get; set; }

        public FrequenciaAlunoPorComponenteCurricularResposta()
        {
            FrequenciasPorBimestre = new List<FrequenciaAlunoPorBimestre>();
        }
    }
}