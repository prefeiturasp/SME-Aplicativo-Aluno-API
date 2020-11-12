using Dapper;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Infra.Persistencia.Repositorios.Base.SGP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TurmaRepositorio : RepositorioSgpComCacheBase, ITurmaRepositorio
    {
        public TurmaRepositorio(ICacheRepositorio cacheRepositorio) 
            : base(cacheRepositorio)
        {
        }

        public async Task<TurmaModalidadeDeEnsinoDto> ObterModalidadeDeEnsinoAsync(string codigoTurma)
        {
            try
            {
                var chaveCache = ChaveCacheTurma(codigoTurma);

                var frequenciaAluno = await _cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(frequenciaAluno))
                    return JsonConvert.DeserializeObject<TurmaModalidadeDeEnsinoDto>(frequenciaAluno);

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

                await _cacheRepositorio.SalvarAsync(chaveCache, turmaModalidadeDeEnsino, 720, false);
                return turmaModalidadeDeEnsino;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        private static string ChaveCacheTurma(string codigoTurma)
            => $"modalidade-ensino-turma-{codigoTurma}";
    }
}
