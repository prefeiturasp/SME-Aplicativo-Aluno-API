using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Infra.Persistencia.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoCorRepositorio : RepositorioAppComCacheBase, IFrequenciaAlunoCorRepositorio
    {
        private const string ChaveCache = "frequenciaAlunoCores";

        public FrequenciaAlunoCorRepositorio(ICacheRepositorio cacheRepositorio) 
            : base(cacheRepositorio)
        {
        }

        public async Task<IEnumerable<FrequenciaAlunoCor>> ObterAsync()
        {
            try
            {
                var notasAluno = await _cacheRepositorio.ObterAsync(ChaveCache);
                if (!string.IsNullOrWhiteSpace(notasAluno))
                    return JsonConvert.DeserializeObject<IEnumerable<FrequenciaAlunoCor>>(notasAluno);
                using var conexao = CriaConexao();
                conexao.Open();
                var dadosFrequenciasAluno = await conexao.QueryAsync<FrequenciaAlunoCor>(@"SELECT
                                                                                            frequencia AS Frequencia,
                                                                                            cor AS Cor
                                                                                        FROM public.frequencia_aluno_cor");
                conexao.Close();
                await _cacheRepositorio.SalvarAsync(ChaveCache, dadosFrequenciasAluno, 720, false);
                return dadosFrequenciasAluno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex; throw ex;
            }
        }
    }
}
