﻿namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class DadosLeituraComunicadosPorModalidadeTurmaResultado
    {
        public string NomeAbreviadoDre { get; set; }
        public string Modalidade { get; set; }
        public string CodigoTurma { get; set; }
        public string Turma { get; set; }
        public long NaoReceberamComunicado { get; set; }
        public long ReceberamENaoVisualizaram { get; set; }
        public long VisualizaramComunicado { get; set; }
    }
}
