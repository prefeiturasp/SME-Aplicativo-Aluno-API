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

        public async Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> ObterDadosLeituraModalidade(string codigoDre, string codigoUe, long notificacaoId, bool porResponsavel)
        {
            var sqlResponsavel =
                @"
                select
	                da.dre_nome NomeAbreviadoDre,
	                cn.modalidade_codigo Modalidade,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_responsaveis_sem_app NaoReceberamComunicado,
	                cn.quantidade_responsaveis_com_app - coalesce(leram, 0) ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,',')) modalidade, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,','))
                ) ul on ul.notificacao_id = cn.notificacao_id and ul.dre_codigoeol::varchar = cn.dre_codigo and ul.ue_codigoeol = cn.ue_codigo and ul.modalidade = cn.modalidade_codigo::text
                where
	                cn.dre_codigo = @codigoDre and
	                cn.ue_codigo = @codigoUe and
	                cn.notificacao_id = @notificacaoId and 
	                cn.turma_codigo = 0 and
	                cn.modalidade_codigo <> 0
                ";
            var sqlAluno =
                @"
                select
	                da.dre_nome NomeAbreviadoDre,
	                cn.modalidade_codigo Modalidade,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_alunos_sem_app NaoReceberamComunicado,
	                cn.quantidade_alunos_com_app - coalesce(leram, 0) ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,',')) modalidade, count(distinct codigo_eol_aluno) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.grupo,','))
                ) ul on ul.notificacao_id = cn.notificacao_id and ul.dre_codigoeol::varchar = cn.dre_codigo and ul.ue_codigoeol = cn.ue_codigo and ul.modalidade = cn.modalidade_codigo::text
                where
	                cn.dre_codigo = @codigoDre and
	                cn.ue_codigo = @codigoUe and
	                cn.notificacao_id = @notificacaoId and 
	                cn.turma_codigo = 0 and
	                cn.modalidade_codigo <> 0
                ";

            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados =
                    await conexao.QueryAsync<DadosLeituraComunicadosPorModalidadeTurmaResultado>(
                        porResponsavel ? sqlResponsavel : sqlAluno,
                        new { notificacaoId, codigoDre, codigoUe }
                        );
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
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

        public async Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicadosPorDre(long notificacaoId)
        {
            try
            {
                var sql = @"SELECT 
                                distinct
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
                            where dre_codigo <> '' 
                            and ue_codigo = '' 
                            and modalidade_codigo = 0 
                            and turma_codigo = 0 
                            and notificacao_id = @notificacaoId ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosConsolidacaoNotificacaoResultado>(sql, new { notificacaoId });
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