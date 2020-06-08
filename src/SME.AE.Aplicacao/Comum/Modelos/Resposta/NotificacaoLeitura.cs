using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
  public  class NotificacaoUsuarioLeitura
    {
        public Notificacao Notificaco { get; set; }
        public bool MensagemLida { get; set; }
    }
}
