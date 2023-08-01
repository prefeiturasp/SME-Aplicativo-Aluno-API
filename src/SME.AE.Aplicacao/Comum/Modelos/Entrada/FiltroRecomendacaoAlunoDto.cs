namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class FiltroRecomendacaoAlunoDto
    {
        public string CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public int AnoLetivo { get; set; }
        public int Modalidade { get; set; }
        public int Semestre { get; set; }
    }
}
