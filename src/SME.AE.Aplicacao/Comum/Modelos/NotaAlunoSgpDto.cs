namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class NotaAlunoSgpDto
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public int Bimestre { get; set; }
        public string CodigoAluno { get; set; }
        public int CodigoComponenteCurricular { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Nota { get; set; }
        public string NotaDescricao { get; set; }
        public string RecomendacoesAluno { get; set; }
        public string RecomendacoesFamilia { get; set; }

    }
}