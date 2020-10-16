using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class AtualizacaoNotificacaoResposta 
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime? DataExpiracao { get; set; }
    }



}
