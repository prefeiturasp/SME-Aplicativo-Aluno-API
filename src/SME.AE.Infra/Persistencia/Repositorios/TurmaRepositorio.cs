using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TurmaRepositorio : ITurmaRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public TurmaRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<TurmaModalidadeDeEnsinoDto> ObterModalidadeDeEnsino(string codigoTurma)
        {
            try
            {
                using var conexao = CriaConexao();

                conexao.Open();

                var query = @"SELECT
                                turma_id AS CodigoDaTurma,
                                modalidade_codigo AS ModalidadeDeEnsino
                            FROM 
                                turma
                            WHERE
                                turma_id = @codigoTurma";

                var parametros = new { codigoTurma };

                var turmaModalidadeDeEnsino = await servicoTelemetria.RegistrarComRetornoAsync<TurmaModalidadeDeEnsinoDto>(async () =>
                    await SqlMapper.QuerySingleAsync<TurmaModalidadeDeEnsinoDto>(conexao, query, parametros), "query", "Query SGP", query, parametros.ToString());

                conexao.Close();

                return turmaModalidadeDeEnsino;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
