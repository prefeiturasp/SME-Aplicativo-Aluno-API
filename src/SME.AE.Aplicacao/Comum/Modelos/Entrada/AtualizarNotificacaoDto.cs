using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class AtualizarNotificacaoDto
    {

        public long Id { get; set; }

        public string Titulo { get; set; }

        public string Mensagem { get; set; }

        public DateTime DataExpiracao { get; set; }

        public string AlteradoPor { get; set; }

        public DateTime AlteradoEm { get; set; }
        public bool EnviadoPushNotification { get; set; }
    }
}
