using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos.Externos
{
    public class UsuarioSenhaHistoricoCoreSSOMap : ExternoMap<UsuarioSenhaHistoricoCoreSSO>
    {
        public UsuarioSenhaHistoricoCoreSSOMap()
        {
            ToTable("SYS_UsuarioSenhaHistorico");
            Map(x => x.UsuarioId).ToColumn("usu_id");
            Map(x => x.SenhaId).ToColumn("ush_id");
            Map(x => x.Criptografia).ToColumn("ush_criptografia");
            Map(x => x.Senha).ToColumn("ush_senha");
            Map(x => x.Data).ToColumn("ush_data");
        }
    }
}
