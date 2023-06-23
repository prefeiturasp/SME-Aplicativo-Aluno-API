using Dapper;
using Dapper.Dommel;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ComponenteCurricularSgpRepositorio : IComponenteCurricularSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public ComponenteCurricularSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<string> ObterDescricaoComponenteCurricular(long codigoComponenteCurricular)
        {
            try
            {
                using (var conexao = CriaConexao())
                {
                    await conexao.OpenAsync();
                    return await conexao.QueryFirstOrDefaultAsync<string>(
                        "select descricao_sgp from componente_curricular cc where cc.id = @codigoComponenteCurricular;", new { codigoComponenteCurricular });
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
