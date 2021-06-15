using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TurmaRepositorio : ITurmaRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

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
                var turmaModalidadeDeEnsino = await conexao.QuerySingleAsync<TurmaModalidadeDeEnsinoDto>(query, parametros);
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
