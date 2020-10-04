using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class EventoRespostaDto
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public int tipo_evento { get; set; }
        public int ano_letivo { get; set; }
    }
}
