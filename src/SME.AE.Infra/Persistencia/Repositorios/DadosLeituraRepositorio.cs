using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class DadosLeituraRepositorio : BaseRepositorio<Adesao>, IDadosLeituraRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;

        public DadosLeituraRepositorio(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe, long notificacaoId)
        {
            try
            {
                var sql = "";
                if (string.IsNullOrEmpty(codigoDre) && string.IsNullOrEmpty(codigoUe))
                    sql = @"SELECT 
                                ano_letivo as AnoLetivo,
                                notificacao_id as NotificacaoId,
                                dre_codigo as DreCodigo,
                                ue_codigo as UeCodigo,
	                            quantidade_alunos_com_app as QuantidadeAlunosComApp,
	                            quantidade_alunos_sem_app as QuantidadeAlunosSemApp,
	                            quantidade_responsaveis_com_app as QuantidadeResponsaveisComApp,
	                            quantidade_responsaveis_sem_app as QuantidadeResponsaveisSemApp,
                                turma as Turma,
                                turma_codigo as TurmaCodigo,
                                modalidade_codigo as ModalidadeCodigo
                            FROM consolidacao_notificacao 
                            where dre_codigo = '' 
                            and ue_codigo = '' 
                            and modalidade_codigo = 0 
                            and turma_codigo = 0 
                            and notificacao_id = @notificacaoId ";

                if (!string.IsNullOrEmpty(codigoDre) && string.IsNullOrEmpty(codigoUe))
                    sql = @"SELECT 
                                ano_letivo as AnoLetivo,
                                notificacao_id as NotificacaoId,
                                dre_codigo as DreCodigo,
                                ue_codigo as UeCodigo,
	                            quantidade_alunos_com_app as QuantidadeAlunosComApp,
	                            quantidade_alunos_sem_app as QuantidadeAlunosSemApp,
	                            quantidade_responsaveis_com_app as QuantidadeResponsaveisComApp,
	                            quantidade_responsaveis_sem_app as QuantidadeResponsaveisSemApp,
                                turma as Turma,
                                turma_codigo as TurmaCodigo,
                                modalidade_codigo as ModalidadeCodigo
                            FROM consolidacao_notificacao 
                            where dre_codigo = @codigoDre 
                            and ue_codigo <> '' 
                            and modalidade_codigo = 0 
                            and turma_codigo = 0 
                            and notificacao_id = @notificacaoId ";

                if (!string.IsNullOrEmpty(codigoDre) && !string.IsNullOrEmpty(codigoUe))
                    sql = @"SELECT 
                                ano_letivo as AnoLetivo,
                                notificacao_id as NotificacaoId,
                                dre_codigo as DreCodigo,
                                ue_codigo as UeCodigo,
	                            quantidade_alunos_com_app as QuantidadeAlunosComApp,
	                            quantidade_alunos_sem_app as QuantidadeAlunosSemApp,
	                            quantidade_responsaveis_com_app as QuantidadeResponsaveisComApp,
	                            quantidade_responsaveis_sem_app as QuantidadeResponsaveisSemApp,
                                turma as Turma,
                                turma_codigo as TurmaCodigo,
                                modalidade_codigo as ModalidadeCodigo
                            FROM consolidacao_notificacao 
                            where dre_codigo = @codigoDre 
                            and ue_codigo = @codigoUe 
                            and modalidade_codigo = 0 
                            and turma_codigo = 0 
                            and notificacao_id = @notificacaoId ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosConsolidacaoNotificacaoResultado>(sql, new { notificacaoId, codigoDre, codigoUe });
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicadosPorDre(long notificaoId)
        {
            try
            {
                var sql = @"select * from consolidacao_notificacao cn where ano_letivo = 2020 and dre_codigo <> '' and ue_codigo = '' ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosConsolidacaoNotificacaoResultado>(sql);
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}