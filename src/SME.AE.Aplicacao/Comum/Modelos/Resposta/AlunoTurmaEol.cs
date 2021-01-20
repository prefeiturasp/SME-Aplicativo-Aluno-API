using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class AlunoTurmaEol
    {
        public string NumeroChamada { get; set; }
        public string NomeAluno { get; set; }
        public long CodigoEOLAluno { get; set; }
        public long Cpf { get; set; }
        public string NomeResponsavel { get; set; }
        public short TipoResponsavel { get; set; }
        public string DDDCelular { get; set; }
        public string Celular { get; set; }
        public string DDDFixo { get; set; }
        public string TelefoneFixo { get; set; }
        public short SituacaoAluno { get; set; }
        public DateTime? DataSituacaoAluno { get; set; }
    }
}
