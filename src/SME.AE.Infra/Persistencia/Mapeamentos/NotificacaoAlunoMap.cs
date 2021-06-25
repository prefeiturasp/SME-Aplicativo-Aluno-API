using Dapper.FluentMap.Dommel.Mapping;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class NotificacaoAlunoMap : DommelEntityMap<NotificacaoAluno>
    {
        public NotificacaoAlunoMap()
        {
            ToTable("notificacao_aluno");

            Map(x => x.CodigoAluno).ToColumn("codigo_eol_aluno");
            Map(x => x.NotificacaoId).ToColumn("notificacao_id");
            Map(x => x.CriadoEm).ToColumn("criadoem");

        }
    }
}
