using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioNotificacaoDto
    {
        public long NotificacaoId { get; set; }
        public long UsuarioId { get; set; }
        public long CodigoAlunoEol { get; set; }
        public bool MensagemVisualizada { get; set; }
    }
}
