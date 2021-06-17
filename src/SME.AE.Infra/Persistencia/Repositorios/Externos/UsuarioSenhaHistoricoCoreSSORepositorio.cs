using Dapper;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades.Externas;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios.Externos
{
    public class UsuarioSenhaHistoricoCoreSSORepositorio : ExternoRepositorio<UsuarioSenhaHistoricoCoreSSO, SqlConnection>, IUsuarioSenhaHistoricoCoreSSORepositorio
    {
        public UsuarioSenhaHistoricoCoreSSORepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions) : base(new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection))
        {
        }

        public async Task<bool> VerificarUltimas5Senhas(Guid usuId, string senha)
        {
            var senhas = await database.Conexao.QueryAsync<UsuarioSenhaHistoricoCoreSSO>(UsuarioSenhaHitoricoConsultas.ObterUltimas5Senhas, new { usuId });

            database.Conexao.Close();

            return senhas.Any(x => x.Senha.Equals(senha));
        }

        public async Task AdicionarSenhaHistorico(UsuarioSenhaHistoricoCoreSSO usuarioSenhaHistoricoCoreSSO)
        {
            var sql = @"INSERT INTO SYS_UsuarioSenhaHistorico
                            (usu_id,ush_senha,ush_criptografia,ush_data)
                            VALUES (@usuid,@ushsenha,@ushcriptografia,@ushdata);";

            await database.Conexao.ExecuteAsync(sql, new
            {
                usuid = usuarioSenhaHistoricoCoreSSO.UsuarioId,
                ushsenha = usuarioSenhaHistoricoCoreSSO.Senha,
                ushcriptografia = usuarioSenhaHistoricoCoreSSO.Criptografia,
                ushdata = usuarioSenhaHistoricoCoreSSO.Data
            });

            database.Conexao.Close();
        }
    }
}
