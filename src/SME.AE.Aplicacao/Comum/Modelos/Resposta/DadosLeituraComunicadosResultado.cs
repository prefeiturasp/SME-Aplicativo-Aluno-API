namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class DadosLeituraComunicadosResultado
    {
        public string NomeAbreviadoDre { get; set; }
        public string NomeAbreviadoUe { get; set; }
        public long NaoReceberamComunicado { get; set; }
        public long ReceberamENaoVisualizaram { get; set; }
        public long VisualizaramComunicado { get; set; }
    }
}
