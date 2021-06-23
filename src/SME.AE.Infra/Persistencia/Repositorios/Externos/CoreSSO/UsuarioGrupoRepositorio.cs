
using Dapper;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios.Externos.CoreSSO
{
    public class UsuarioGrupoRepositorio : ExternoRepositorio<UsuarioGrupoCoreSSO, SqlConnection>, IUsuarioGrupoRepositorio
    {
        public UsuarioGrupoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions) : base(new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection))
        {
        }

        public async Task<IEnumerable<UsuarioGrupoCoreSSO>> ObterPorUsuarioId(Guid usuarioId)
        {
            var sql = "select usu_id as 'UsuarioId', gru_id as 'GrupoId',usg_situacao as 'Situacao' from SYS_UsuarioGrupo where usu_id = @usuarioId";

            var retorno = await database.Conexao.QueryAsync<UsuarioGrupoCoreSSO>(sql, new { usuarioId });

            database.Conexao.Close();

            return retorno;
        }
    }
}
