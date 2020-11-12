using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno
{
    public class FrequenciaAlunoResposta
    {
        public string AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public long QuantidadeAulas { get; set; }
        public long QuantidadeFaltas { get; set; }
        public long QuantidadeCompensacoes { get; set; }
        public decimal Frequencia => ObterFrequencia();
        public string CorDaFrequencia { get; set; }
        public ICollection<ComponenteCurricularDoAluno> ComponentesCurricularesDoAluno { get; set; }

        public FrequenciaAlunoResposta()
        {
            ComponentesCurricularesDoAluno = new List<ComponenteCurricularDoAluno>();
        }

        private decimal ObterFrequencia()
        {
            if (QuantidadeFaltas <= 0) return 100.00m;

            var faltasNaoCompensadas = QuantidadeFaltas - QuantidadeCompensacoes;
            if (QuantidadeFaltas - QuantidadeCompensacoes <= 0) return 100.00m;

            return 100.00m - (faltasNaoCompensadas * 100.00m / QuantidadeAulas);
        }
    }
}