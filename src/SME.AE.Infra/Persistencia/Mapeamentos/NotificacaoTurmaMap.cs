using Dapper.FluentMap.Dommel.Mapping;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class NotificacaoTurmaMap : DommelEntityMap<NotificacaoTurma>
    {
        public NotificacaoTurmaMap()
        {
            ToTable("notificacao_turma");

           // Map(x => x.Id).ToColumn("id").IsKey();
            Map(x => x.CodigoTurma).ToColumn("codigo_eol_turma");
            Map(x => x.NotificacaoId).ToColumn("notificacao_id");
            Map(x => x.CriadoEm).ToColumn("criadoem");

        }
    }
}
