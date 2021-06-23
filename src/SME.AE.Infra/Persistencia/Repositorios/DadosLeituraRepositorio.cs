using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class DadosLeituraRepositorio : BaseRepositorio<Adesao>, IDadosLeituraRepositorio
    {
        public DadosLeituraRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions) : base(variaveisGlobaisOptions.AEConnection)
        {
        }

        public async Task<IEnumerable<DataLeituraAluno>> ObterDadosLeituraAlunos(long notificacaoId, string codigosAlunos)
        {
            var sql =
                @$"
                select 
	                codigo_eol_aluno CodigoAluno,
	                greatest(criadoem, alteradoem) DataLeitura
                from 
	                usuario_notificacao_leitura unl
                where 
	                notificacao_id = {notificacaoId} and 
	                codigo_eol_aluno in ({codigosAlunos})
                ";
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados =
                    await conexao.QueryAsync<DataLeituraAluno>(sql);
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> ObterDadosLeituraTurma(string codigoDre, string codigoUe, long notificacaoId, short[] modalidades, long[] codigosTurmas, bool porResponsavel)
        {

            var sqlResponsavel = new StringBuilder(@"
                select
	                da.dre_nome NomeAbreviadoDre,
	                cn.modalidade_codigo ModalidadeCodigo,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_responsaveis_sem_app NaoReceberamComunicado,
                    CASE 
	                when cn.quantidade_alunos_com_app = 0 THEN
	                cn.quantidade_alunos_com_app
	                when cn.quantidade_alunos_com_app > 0 THEN
	                cn.quantidade_alunos_com_app - coalesce(leram, 0)
	                end ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, unl.codigo_eol_turma, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                left join notificacao_turma nt on nt.notificacao_id = unl.notificacao_id 
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')), unl.codigo_eol_turma 
                ) ul on ul.notificacao_id = cn.notificacao_id and 
                  ul.dre_codigoeol::varchar = cn.dre_codigo and 
                  ul.ue_codigoeol = cn.ue_codigo and 
                  --ul.modalidade = cn.modalidade_codigo::text and 
                  ul.codigo_eol_turma = cn.turma_codigo 
                where
	                cn.dre_codigo = @codigoDre and
	                cn.ue_codigo = @codigoUe and
	                cn.notificacao_id = @notificacaoId 
                ");

            if (modalidades != null && modalidades.Any())
            {
                sqlResponsavel.Append(" and cn.modalidade_codigo = ANY(@modalidades) ");
            }
            else
            {
                sqlResponsavel.Append(" and cn.modalidade_codigo <> 0");
            }

            if (codigosTurmas != null && codigosTurmas.Any())
            {
                sqlResponsavel.Append(" and cn.turma_codigo = ANY(@codigosTurmas)");
            }
            else
            {
                sqlResponsavel.Append(" and cn.turma_codigo <> 0");
            }

            var sqlAluno = new StringBuilder(@"
                select
	                da.dre_nome NomeAbreviadoDre,
	                cn.modalidade_codigo ModalidadeCodigo,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_alunos_sem_app NaoReceberamComunicado,
                    CASE 
	                when cn.quantidade_alunos_com_app = 0 THEN
	                cn.quantidade_alunos_com_app
	                when cn.quantidade_alunos_com_app > 0 THEN
	                cn.quantidade_alunos_com_app - coalesce(leram, 0)
	                end ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, unl.codigo_eol_turma, count(distinct codigo_eol_aluno) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                left join notificacao_turma nt on nt.notificacao_id = unl.notificacao_id 
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')), unl.codigo_eol_turma 
                ) ul on ul.notificacao_id = cn.notificacao_id and 
                  ul.dre_codigoeol::varchar = cn.dre_codigo and 
                  ul.ue_codigoeol = cn.ue_codigo and 
                  --ul.modalidade = cn.modalidade_codigo::text and 
                  ul.codigo_eol_turma = cn.turma_codigo 
                where
	                cn.dre_codigo = @codigoDre and
	                cn.ue_codigo = @codigoUe and
	                cn.notificacao_id = @notificacaoId 
                ");

            if (modalidades != null && modalidades.Any())
            {
                sqlResponsavel.Append(" and cn.modalidade_codigo = ANY(@modalidades) ");
            }
            else
            {
                sqlResponsavel.Append(" and cn.modalidade_codigo <> 0");
            }

            if (codigosTurmas != null && codigosTurmas.Any())
            {
                sqlAluno.Append(" and cn.turma_codigo = ANY(@codigosTurmas)");
            }
            else
            {
                sqlAluno.Append(" and cn.turma_codigo <> 0");
            }

            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados =
                    await conexao.QueryAsync<DadosLeituraComunicadosPorModalidadeTurmaResultado>(
                        porResponsavel ? sqlResponsavel.ToString() : sqlAluno.ToString(),
                        new { notificacaoId, codigoDre, codigoUe, modalidades, codigosTurmas }
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
        public async Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> ObterDadosLeituraModalidade(string codigoDre, string codigoUe, long notificacaoId, bool porResponsavel)
        {
            var sqlResponsavel =
                @"
                select
	                da.dre_nome NomeAbreviadoDre,
	                cn.modalidade_codigo ModalidadeCodigo,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_responsaveis_sem_app NaoReceberamComunicado,
	                cn.quantidade_responsaveis_com_app - coalesce(leram, 0) ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, count(distinct usuario_cpf) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,','))
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
	                cn.modalidade_codigo ModalidadeCodigo,
	                cn.turma_codigo CodigoTurma,
	                cn.turma,
	                cn.quantidade_alunos_sem_app NaoReceberamComunicado,
	                cn.quantidade_alunos_com_app - coalesce(leram, 0) ReceberamENaoVisualizaram,
	                coalesce(leram, 0) VisualizaramComunicado 
                from consolidacao_notificacao cn
                left join (select distinct dre_codigo, dre_nome from dashboard_adesao) da on da.dre_codigo = cn.dre_codigo
                left join 
                (
	                select unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,',')) modalidade, count(distinct codigo_eol_aluno) leram
	                from usuario_notificacao_leitura unl 
	                left join notificacao n on n.id = unl.notificacao_id
	                group by unl.notificacao_id, unl.dre_codigoeol, unl.ue_codigoeol, unnest(string_to_array(n.modalidades,','))
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

        public async Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe, long notificacaoId, short modalidade)
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

                if (!string.IsNullOrEmpty(codigoDre) && !string.IsNullOrEmpty(codigoUe) && modalidade == 0)
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

                if (!string.IsNullOrEmpty(codigoDre) && !string.IsNullOrEmpty(codigoUe) && modalidade > 0)
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
                            and modalidade_codigo <> 0
                            and turma_codigo = 0 
                            and notificacao_id = @notificacaoId ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosConsolidacaoNotificacaoResultado>(sql, new { notificacaoId, codigoDre, codigoUe, modalidade });
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