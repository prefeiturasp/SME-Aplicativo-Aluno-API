using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UeSgpRepositorio : IUeSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public UeSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<long[]> ObterIdUes()
        {
            try
            {
                using (var conexao = CriaConexao())
                {
                    await conexao.OpenAsync();
                    var query = "select id from ue order by id;";
                    return (await conexao.QueryAsync<long>(query)).ToArray();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
