using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class NotificacaoSgpDto : ModeloBase
    {
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public string Grupo { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
    }
}
