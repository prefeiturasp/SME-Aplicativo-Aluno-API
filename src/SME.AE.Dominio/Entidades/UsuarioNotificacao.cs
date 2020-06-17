using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
    [Table("usuario_notificacao_leitura")]
    public class UsuarioNotificacao : EntidadeBase
    {
        public long UsuarioId { get; set; }
        public long CodigoAlunoEol { get; set; }
        public long NotificacaoId { get; set; }
    }
}
