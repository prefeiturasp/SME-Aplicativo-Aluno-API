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
        private readonly ICacheRepositorio _cacheRepositorio;
        private const string ChaveCache = "notaAlunoCores";

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

        public NotaAlunoCorRepositorio(ICacheRepositorio cacheRepositorio)
        {
            _cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<NotaAlunoCor>> ObterAsync()
        {
            try
            {
                var notasAluno = await _cacheRepositorio.ObterAsync(ChaveCache);
                if (!string.IsNullOrWhiteSpace(notasAluno))
                    return JsonConvert.DeserializeObject<IEnumerable<NotaAlunoCor>>(notasAluno);

                using var conexao = CriaConexao();
                conexao.Open();
                var dadosNotasAluno = await conexao.QueryAsync<NotaAlunoCor>(@"SELECT
                                                                                nota AS Nota,
                                                                                cor AS Cor
                                                                             FROM public.nota_aluno_cor");
                conexao.Close();

                await _cacheRepositorio.SalvarAsync(ChaveCache, dadosNotasAluno, 720, false);
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