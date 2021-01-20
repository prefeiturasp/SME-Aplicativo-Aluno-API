using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class EventoRespostaDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string DiaSemana { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int TipoEvento { get; set; }
        public int AnoLetivo { get; set; }
        public string ComponenteCurricular { get; set; }
    }
}
