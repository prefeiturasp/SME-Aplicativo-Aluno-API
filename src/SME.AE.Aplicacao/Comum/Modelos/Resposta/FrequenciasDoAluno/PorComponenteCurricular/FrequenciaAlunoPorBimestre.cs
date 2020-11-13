using System;
using System.Collections.Generic;
using System.Linq;

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
        public string DiasAusencia { set => SetDiasDeAusencia(value); }
        public IEnumerable<FrequenciaAlunoPorBimestreAusencia> Ausencias { get; private set; }

        public FrequenciaAlunoPorBimestre()
        {
            Ausencias = new List<FrequenciaAlunoPorBimestreAusencia>();
        }

        private decimal ObterFrequencia()
        {
            if (QuantidadeFaltas <= 0) return 100.00m;

            var faltasNaoCompensadas = QuantidadeFaltas - QuantidadeCompensacoes;
            if (QuantidadeFaltas - QuantidadeCompensacoes <= 0) return 100.00m;

            return 100.00m - (faltasNaoCompensadas * 100.00m / QuantidadeAulas);
        }

        private void SetDiasDeAusencia(string diasAusenciaConcatenado)
        {
            if (string.IsNullOrWhiteSpace(diasAusenciaConcatenado)) return;
            var diasAusenciaComNumeroDaAula = diasAusenciaConcatenado.Split(",");

            var diasAusencia = diasAusenciaComNumeroDaAula
                .Select(x =>
                {
                    var diaAusenciaSegmentado = x.Split(":");
                    return DateTime.TryParse(diaAusenciaSegmentado[0], out var data) ? data : (DateTime?)null;
                }).ToList();

            Ausencias = diasAusencia
                .Where(x => x != null)
                .GroupBy(x => x)
                .Select(x => new FrequenciaAlunoPorBimestreAusencia
                {
                    Data = x.Key.GetValueOrDefault(),
                    QuantidadeDeFaltas = x.Count()
                })
                .OrderBy(x => x.Data)
                .ToList();
        }
    }
}