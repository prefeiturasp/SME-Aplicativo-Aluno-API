using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoRepositorio : IFrequenciaAlunoRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

        public FrequenciaAlunoRepositorio(ICacheRepositorio cacheRepositorio) 
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<FrequenciaAlunoResposta>> ObterFrequenciaAluno(string codigoUe, long codigoTurma, string codigoAluno)
        {
            try
            {
                var chaveCache = $"frequenciaAluno";
                var frequenciaAluno = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(frequenciaAluno))
                    return JsonConvert.DeserializeObject<IEnumerable<FrequenciaAlunoResposta>>(frequenciaAluno);

                using var conexao = CriaConexao();
                conexao.Open();
                var dadosFrequenciaAlunos = await conexao.QueryAsync<FrequenciaAlunoResposta>(@"SELECT 
                                                                                                        ue_codigo as CodigoUe,
                                                                                                        ue_nome as NomeUe,
                                                                                                        turma_codigo as CodigoTurma,
                                                                                                        turma_descricao as NomeTurma,
	                                                                                                    aluno_codigo as AlunoCodigo,
                                                                                                        bimestre as Bimestre,
                                                                                                        componente_curricular as ComponenteCurricular,
                                                                                                        quantidade_aulas as QuantidadeAulas,
                                                                                                        quantidade_faltas as QuantidadeFaltas,
                                                                                                        quantidade_compensacoes as QuantidadeCompensacoes
                                                                                                    FROM public.frequencia_aluno 
                                                                                                    WHERE 
                                                                                                        ue_codigo = @CodigoUe 
                                                                                                        AND turma_codigo = @CodigoTurma 
                                                                                                        AND aluno_codigo = @CodigoAluno ", new {  codigoUe, codigoTurma, codigoAluno });
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosFrequenciaAlunos, 720, false);
                return dadosFrequenciaAlunos;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}