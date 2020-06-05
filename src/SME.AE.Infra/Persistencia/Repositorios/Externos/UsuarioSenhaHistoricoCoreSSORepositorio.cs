using Dapper;
using Dapper.Contrib.Extensions;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Dominio.Entidades.Externas;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios.Externos
{
    public class UsuarioSenhaHistoricoCoreSSORepositorio : ExternoRepositorio<UsuarioSenhaHistoricoCoreSSO, SqlConnection>, IUsuarioSenhaHistoricoCoreSSORepositorio
    {
        protected UsuarioSenhaHistoricoCoreSSORepositorio() : base(new SqlConnection(ConnectionStrings.ConexaoCoreSSO))
        {
        }

        public async Task<bool> VerificarUltimas5Senhas(string usuId, string senha)
        {
            var senhas = await database.Conexao.QueryAsync<UsuarioSenhaHistoricoCoreSSO>(UsuarioSenhaHitoricoConsultas.ObterUltimas5Senhas, new { usuId });

            return senhas.Any(x => x.Senha.Equals(senha));
        }
    }
}
