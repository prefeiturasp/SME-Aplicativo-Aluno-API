using SME.AE.Aplicacao.Comum.Enumeradores;
using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class AlunoRespostaEol
    {
        public int CodigoEol { get; set; }
        public string NomeResponsavel { get; set; }
        public string CpfResponsavel { get; set; }
        public string Nome { get; set; }
        public string NomeSocial { get; set; }
        public string CodigoEscola { get; set; }
        public string CodigoDre { get; set; }
        public string Escola { get; set; }
        public TipoResponsavelEnum TipoResponsavel { get; set; }
        public int CodigoTipoEscola { get; set; }
        public string DescricaoTipoEscola { get; set; }
        public string SiglaDre { get; set; }
        public int CodigoTurma { get; set; }
        public string Turma { get; set; }
        public string SituacaoMatricula { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataSituacaoMatricula { get; set; }
        public int CodigoCicloEnsino { get; set; }
        public int CodigoEtapaEnsino { get; set; }
        public string Grupo { get; set; }
        public long CodigoGrupo { get; internal set; }
        public string SerieResumida { get; set; }
        public int CodigoModalidadeTurma { get; set; }
    }
}
