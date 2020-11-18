using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoCorRepositorio : INotaAlunoCorRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

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