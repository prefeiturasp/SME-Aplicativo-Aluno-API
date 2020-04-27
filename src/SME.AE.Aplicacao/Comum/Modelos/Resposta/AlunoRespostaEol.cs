using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class AlunoRespostaEol
    {
        public int CodigoEol { get; set; }
        public string Nome { get; set; }
        public string NomeSocial { get; set; }
        public string Escola { get; set; }
        public int CodigoTipoEscola { get; set; }
        public string DescricaoTipoEscola { get; set; }
        public string SiglaDre { get; set; }
        public string Turma { get; set; }
        public string SituacaoMatricula { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataSituacaoMatricula { get; set; }
        public string Grupo { get; set; }
    }
}
