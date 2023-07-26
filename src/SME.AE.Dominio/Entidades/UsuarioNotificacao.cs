namespace SME.AE.Dominio.Entidades
{
    public class UsuarioNotificacao : EntidadeBase
    {
        public long UsuarioId { get; set; }
        public long CodigoEolAluno { get; set; }
        public long CodigoEolTurma { get; set; }
        public long NotificacaoId { get; set; }
        public long DreCodigoEol { get; set; }
        public string UeCodigoEol { get; set; }
        public string UsuarioCpf { get; set; }
        public bool MensagemVisualizada { get; set; }
        public bool MensagemExcluida { get; set; }
    }
}
