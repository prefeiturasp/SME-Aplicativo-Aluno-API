using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class StatusNotificacaoUsuario 
    {
        public long NotificacaoId { get; set; }
        public DateTime? DataHoraLeitura { get; set; }
        public string  Situacao { get; set; }

        public StatusNotificacaoUsuario(long notificacaoId, DateTime? dataHoraLeitura, string situacao)
        {
            NotificacaoId = notificacaoId;
            DataHoraLeitura = dataHoraLeitura;
            Situacao = situacao;
        }
    }

}
