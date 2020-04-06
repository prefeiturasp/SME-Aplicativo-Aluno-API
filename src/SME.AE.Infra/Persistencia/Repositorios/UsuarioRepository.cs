
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            Usuario usuario;

            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.ConexaoEol))
                {
                    conn.Open();

                    var resultado = await conn.QueryAsync<Usuario>(UsuarioConsultas.ObterPorCpf, new
                    {
                        Cpf = cpf
                    });
                    usuario = resultado.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }

            return usuario;
        }
    }
}
