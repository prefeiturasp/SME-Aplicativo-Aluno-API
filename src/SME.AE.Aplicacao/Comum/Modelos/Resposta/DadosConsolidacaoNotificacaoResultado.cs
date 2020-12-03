namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class DadosConsolidacaoNotificacaoResultado
    {
        public int AnoLetivo { get; set; }
        public long NotificacaoId { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long QuantidadeResponsaveisComApp { get; set; }
        public long QuantidadeAlunosComApp { get; set; }
        public long QuantidadeResponsaveisSemApp { get; set; }
        public long QuantidadeAlunosSemApp { get; set; }
        public string Turma { get; set; }
        public long TurmaCodigo { get; set; }
        public long ModalidadeCodigo { get; set; }
    }

}
