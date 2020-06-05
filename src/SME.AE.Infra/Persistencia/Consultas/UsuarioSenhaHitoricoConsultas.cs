using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioSenhaHitoricoConsultas
    {
        public static readonly string ObterUltimas5Senhas =
                        @"select TOP 5 * from SYS_UsuarioSenhaHistorico where usu_id = @usuId
                        order by ush_data desc;";
    }
}
