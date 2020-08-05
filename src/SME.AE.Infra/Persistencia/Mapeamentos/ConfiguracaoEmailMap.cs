using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class ConfiguracaoEmailMap : BaseMap<ConfiguracaoEmail>
    {
        public ConfiguracaoEmailMap()
        {
            ToTable("configuracao_email");
            Map(x => x.EmailRemetente).ToColumn("email_remetente");
            Map(x => x.NomeRemetente).ToColumn("nome_remetente");
            Map(x => x.Porta).ToColumn("porta");
            Map(x => x.Senha).ToColumn("senha");
            Map(x => x.ServidorSmtp).ToColumn("servidor_smtp");
            Map(x => x.UsarTls).ToColumn("tls");
            Map(x => x.Usuario).ToColumn("usario");
        }
    }
}
