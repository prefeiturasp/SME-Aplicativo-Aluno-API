namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class ComunicadoSgpDto
    {
        public long Id { get; set; }
        public short AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string TurmaCodigo { get; set; }
        public string AlunoCodigo { get; set; }
        public short? Modalidade { get; set; }
        public string SeriesResumidas { get; set; }
        public short TipoComunicado { get; set; }
        public string TipoEscola { get; set; }
    }
}
