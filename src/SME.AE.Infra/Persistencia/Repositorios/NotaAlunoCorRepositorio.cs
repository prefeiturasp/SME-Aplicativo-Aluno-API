using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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

        public NotaAlunoCorRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<IEnumerable<NotaAlunoCor>> ObterAsync()
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();
                var dadosNotasAluno = await conexao.QueryAsync<NotaAlunoCor>(@"SELECT
                                                                                nota AS Nota,
                                                                                cor AS Cor
                                                                             FROM public.nota_aluno_cor");
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