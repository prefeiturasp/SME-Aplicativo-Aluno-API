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
                var chaveCache = $"dadosAlunos-{cpf}";
                var dadosAlunos = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dadosAlunos))
                    return JsonConvert.DeserializeObject<List<AlunoRespostaEol>>(dadosAlunos);

                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                IEnumerable<AlunoRespostaEol> listaAlunos = await conexao.QueryAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos} {whereReponsavelAluno}", new { cpf });

                await cacheRepositorio.SalvarAsync(chaveCache, listaAlunos, 1080, false);

                return listaAlunos.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}