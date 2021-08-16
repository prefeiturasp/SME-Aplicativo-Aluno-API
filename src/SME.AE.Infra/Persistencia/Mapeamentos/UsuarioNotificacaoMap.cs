using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class UsuarioNotificacaoMap : BaseMap<UsuarioNotificacao>
    {
        public UsuarioNotificacaoMap()
        {
            ToTable("usuario_notificacao_leitura");
            Map(a => a.UsuarioId).ToColumn("usuario_id");
            Map(a => a.CodigoEolAluno).ToColumn("codigo_eol_aluno");
            Map(a => a.CodigoEolTurma).ToColumn("codigo_eol_turma");
            Map(a => a.NotificacaoId).ToColumn("notificacao_id");
            Map(a => a.DreCodigoEol).ToColumn("dre_codigoeol");
            Map(a => a.UeCodigoEol).ToColumn("ue_codigoeol");
            Map(a => a.UsuarioCpf).ToColumn("usuario_cpf");
            Map(a => a.MensagemVisualizada).ToColumn("mensagemvisualizada");
            Map(a => a.MensagemExcluida).ToColumn("mensagemexcluida");
        }
    }
}
