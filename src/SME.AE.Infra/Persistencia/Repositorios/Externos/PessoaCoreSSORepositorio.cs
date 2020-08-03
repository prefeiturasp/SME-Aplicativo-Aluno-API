using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios.Externos
{
    public class PessoaCoreSSORepositorio : ExternoRepositorio<PessoaCoreSSO, SqlConnection>,IPessoaCoreSSORepositorio
    {
        public PessoaCoreSSORepositorio() : base(new SqlConnection(ConnectionStrings.ConexaoCoreSSO)) { }

        public async Task<Guid> Inserir()
        {
            return new Guid();
        }
    }
}
