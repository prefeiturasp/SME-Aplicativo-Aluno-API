using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos.Externos
{
    public class PessoaCoreSSOMap : ExternoMap<PessoaCoreSSO>
    {
        public PessoaCoreSSOMap()
        {
            Map(m => m.Id).ToColumn("pes_id");
            Map(m => m.Integridade).ToColumn("pes_integridade");
            Map(m => m.Nome).ToColumn("pes_nome");
            Map(m => m.Situacao).ToColumn("pes_situacao");
            Map(m => m.DataCriacao).ToColumn("pes_dataCriacao");
            Map(m => m.DataAlteracao).ToColumn("pes_dataAlteracao");
            Map(m => m.NomeSocial).ToColumn("pes_nomeSocial");

        }
    }
}
