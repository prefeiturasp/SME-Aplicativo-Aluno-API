using Dapper;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {

        private readonly ICacheRepositorio cacheRepositorio;

        public AlunoRepositorio(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        private readonly string whereReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";
        public async Task<List<AlunoRespostaEol>> ObterDadosAlunos(string cpf)
        {
            try
            {
                var cache = ObterDadosAlunosCache(cpf);
                if(cache.Result != null && cache.Result.Any())
                {
                    return cache.Result;
                }

                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                IEnumerable<AlunoRespostaEol> listaAlunos = await conexao.QueryAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos} {whereReponsavelAluno}", new { cpf });
                await SalvarDadosAlunosCache(cpf);
                return listaAlunos.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<List<AlunoRespostaEol>> ObterDadosAlunosCache(string cpf) {

            var chaveCache = $"Alunos-{cpf}";
            var alunos = await cacheRepositorio.ObterAsync(chaveCache);
            if (!string.IsNullOrWhiteSpace(alunos))
                return JsonConvert.DeserializeObject<List<AlunoRespostaEol>>(alunos);

            return null;
        }

        public async Task SalvarDadosAlunosCache(string cpf)
        {
            var dadosAluno = this.ObterDadosAlunos(cpf);
            var chaveCache = $"Alunos-{cpf}-{dadosAluno}";
            await cacheRepositorio.SalvarAsync(chaveCache, dadosAluno, 1080, false);
        }


    }
}