namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class ObterFrequenciaAlunoPorComponenteCurricularDto
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public short CodigoComponenteCurricular { get; set; }
    }
}