namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class DadosLeituraResultado
    {
        public string NomeCompletoDre { get; set; }
        public string NomeCompletoUe { get; set; }
        public long NaoReceberamComunicado { get; set; }
        public long ReceberamENaoVisualizaram { get; set; }
        public long VisualizaramComunicado { get; set; }
    }

}
