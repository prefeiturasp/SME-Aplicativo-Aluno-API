using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class FrequenciaAlunoSgpDto
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public string CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public string CodigoAluno { get; set; }
        public int Bimestre { get; set; }
        public long CodigoComponenteCurricular { get; set; }
        public string ComponenteCurricular { get; set; }
        public string DiasAusencias { get; set; }
        public int QuantidadeAulas { get; set; }
        public int QuantidadeAusencias { get; set; }
        public int QuantidadeCompensacoes { get; set; }
    }
}
