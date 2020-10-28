namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class FrequenciaAlunoResposta
    {
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public string CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public int Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public long QuantidadeAulas { get; set; }
        public long QuantidadeFaltas { get; set; }
        public long QuantidadeCompensacoes { get; set; }
    }
}