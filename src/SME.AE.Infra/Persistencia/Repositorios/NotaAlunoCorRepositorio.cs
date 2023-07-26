using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoCorRepositorio : INotaAlunoCorRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public NotaAlunoCorRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<IEnumerable<NotaAlunoCor>> ObterAsync()
        {
            var sql = @"SELECT
                            nota AS Nota,
                            cor AS Cor
                            FROM public.nota_aluno_cor";

            try
            {
                using var conexao = CriaConexao();

                conexao.Open();

                var dadosNotasAluno = await servicoTelemetria.RegistrarComRetornoAsync<NotaAlunoCor>(async () => await SqlMapper.QueryAsync<NotaAlunoCor>(conexao, sql),
                    "query", "Query AE", sql);

                conexao.Close();

                return dadosNotasAluno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}