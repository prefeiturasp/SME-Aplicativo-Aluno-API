using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioNotificacao
    {
        public long notificacaoId { get; set; }
        public string cpfUsuario { get; set; }
        public bool  mensagemVisualizada { get; set; }
    }
}
