using Dapper.FluentMap.Dommel.Mapping;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class NotificacaoTurmaMap : DommelEntityMap<NotificacaoTurma>
    {
        public NotificacaoTurmaMap()
        {
            ToTable("notificacao_turma");

            Map(x => x.CodigoTurma).ToColumn("codigo_eol_turma");
            Map(x => x.NotificacaoId).ToColumn("notificacao_id");
            Map(x => x.CriadoEm).ToColumn("criadoem");

        }
    }
}
