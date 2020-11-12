using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular
{
    public class FrequenciaAlunoPorBimestre
    {
        public int Bimestre { get; set; }
        public long QuantidadeAulas { get; set; }
        public long QuantidadeFaltas { get; set; }
        public long QuantidadeCompensacoes { get; set; }
        public string CorDaFrequencia { get; set; }
        public decimal Frequencia => ObterFrequencia();

        private decimal ObterFrequencia()
        {
            if (QuantidadeFaltas <= 0) return 100.00m;
            
            var faltasNaoCompensadas = QuantidadeFaltas - QuantidadeCompensacoes;
            if (QuantidadeFaltas - QuantidadeCompensacoes <= 0) return 100.00m;

            return 100.00m - (faltasNaoCompensadas*100.00m/QuantidadeAulas);
        }
    }
}