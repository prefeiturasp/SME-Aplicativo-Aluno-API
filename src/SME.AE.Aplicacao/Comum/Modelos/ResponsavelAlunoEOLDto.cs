namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class ResponsavelAlunoEOLDto
    {
        public string CodigoDre { get; set; }
        public string Dre { get; set; }
        public string CodigoUe { get; set; }
        public string Ue { get; set; }
        public long CodigoTurma { get; set; }
        public string Turma { get; set; }
        public long CpfResponsavel { get; set; }
        public long CodigoAluno { get; set; }
        public short CodigoTipoEscola { get; set; }
        public short CodigoEtapaEnsino { get; set; }
        public short CodigoCicloEnsino { get; set; }
        public string SerieResumida { get; set; }
        public short CodigoModalidadeTurma { get; set; }
        public bool TemAppInstalado { get; set; }
    }
}

