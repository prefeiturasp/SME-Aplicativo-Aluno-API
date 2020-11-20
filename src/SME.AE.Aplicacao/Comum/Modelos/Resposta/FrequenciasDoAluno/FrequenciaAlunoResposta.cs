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

        /// <summary>
        /// Realiza o cálculo de frequencia do aluno baseado nas quantidades de aulas, faltas e compensações.
        /// O cálculo resulta em um decimal entre 0.0 (0%) e 1.0 (100%).
        /// </summary>
        private decimal ObterFrequencia()
        {
            if (QuantidadeFaltas <= 0) return 1.00m;

            decimal faltasNaoCompensadas = QuantidadeFaltas - QuantidadeCompensacoes;
            if (QuantidadeFaltas - QuantidadeCompensacoes <= 0) return 1.00m;

            return 1.00m - (faltasNaoCompensadas / (decimal)QuantidadeAulas);
        }
    }
}