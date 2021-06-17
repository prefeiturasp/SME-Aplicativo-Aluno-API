using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ConsolidarLeituraNotificacaoRepositorio : IConsolidarLeituraNotificacaoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public ConsolidarLeituraNotificacaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<IEnumerable<UsuarioAlunoNotificacaoApp>> ObterUsuariosAlunosNotificacoesApp()
        {
            const string sql =
                @"
                select distinct 
	                coalesce(ano_letivo, 0) AnoLetivo,
	                coalesce(cpf, '0')::int8 CpfResponsavel, 
	                coalesce(codigo_eol_aluno::varchar, '') CodigoAluno, 
	                coalesce(notificacao_id, 0) NotificacaoId 
                from 
	                usuario u
                left join 
                (
	                select notificacao_id, codigo_eol_aluno, usuario_cpf, n.ano_letivo
	                from usuario_notificacao_leitura unl
	                inner join notificacao n on n.id = unl.notificacao_id
	                where (ano_letivo >= 2020 and ano_letivo >= date_part('year', current_date)-1) 
                ) lei on lei.usuario_cpf = u.cpf
                where 
	                not u.primeiroacesso and not u.excluido
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var usuariosAlunosNotificacoes = await conn.QueryAsync<UsuarioAlunoNotificacaoApp>(sql);
                conn.Close();
                return usuariosAlunosNotificacoes;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task SalvarConsolidacaoNotificacoesEmBatch(IEnumerable<ConsolidacaoNotificacaoDto> consolidacaoNotificacoes)
        {
            try
            {
                consolidacaoNotificacoes
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(async consolidacaoNotificacao => await SalvarConsolidacaoNotificacao(consolidacaoNotificacao));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            await Task.CompletedTask;
        }
        public async Task SalvarConsolidacaoNotificacao(ConsolidacaoNotificacaoDto consolidacaoNotificacao)
        {
            const string sqlDelete =
                @"
                    DELETE FROM 
	                    consolidacao_notificacao 
                    WHERE 
	                    notificacao_id=@NotificacaoId 
                    AND dre_codigo=@DreCodigo
                    AND ue_codigo=@UeCodigo
                    and modalidade_codigo=@ModalidadeCodigo
                    and turma_codigo=@TurmaCodigo
                    AND ano_letivo=@AnoLetivo
                ";

            const string sqlUpdate =
                @"
                update consolidacao_notificacao
                set
	                quantidade_alunos_sem_app = @QuantidadeAlunosSemApp,
	                quantidade_responsaveis_sem_app = @QuantidadeResponsaveisSemApp,
	                quantidade_alunos_com_app = @QuantidadeAlunosComApp,
	                quantidade_responsaveis_com_app = @QuantidadeResponsaveisComApp 
                where 
	                notificacao_id=@NotificacaoId 
                and dre_codigo=@DreCodigo
                and ue_codigo=@UeCodigo
                and modalidade_codigo=@ModalidadeCodigo
                and turma_codigo=@TurmaCodigo
                and ano_letivo=@AnoLetivo
                ";

            const string sqlInsert =
                @"
                insert into consolidacao_notificacao 
                (
                    ano_letivo,
	                notificacao_id,
	                dre_codigo,
	                ue_codigo,
                    modalidade_codigo,
                    turma_codigo,
                    turma,
	                quantidade_responsaveis_sem_app,
	                quantidade_alunos_sem_app,
	                quantidade_responsaveis_com_app,
	                quantidade_alunos_com_app
                ) values (
                    @AnoLetivo,
	                @NotificacaoId,
	                @DreCodigo,
	                @UeCodigo,
                    @ModalidadeCodigo,
                    @TurmaCodigo,
                    @Turma,
	                @QuantidadeResponsaveisSemApp,
	                @QuantidadeAlunosSemApp,
	                @QuantidadeResponsaveisComApp,
	                @QuantidadeAlunosComApp
                )
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var alterado = (await conn.ExecuteAsync(sqlUpdate, consolidacaoNotificacao));
                if (alterado == 0)
                {
                    await conn.ExecuteAsync(sqlInsert, consolidacaoNotificacao);
                }
                else if (alterado > 1)
                {
                    await conn.ExecuteAsync(sqlDelete, consolidacaoNotificacao);
                    await conn.ExecuteAsync(sqlInsert, consolidacaoNotificacao);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
        public async Task ExcluirConsolidacaoNotificacao(ConsolidacaoNotificacaoDto consolidacaoNotificacao)
        {
            const string sqlDelete =
                @"
                    DELETE FROM 
	                    consolidacao_notificacao 
                    WHERE 
	                    notificacao_id=@NotificacaoId 
                    AND dre_codigo=@DreCodigo
                    AND ue_codigo=@UeCodigo
                    and modalidade_codigo=@ModalidadeCodigo
                    and turma_codigo=@TurmaCodigo
                    AND ano_letivo=@AnoLetivo
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                await conn.ExecuteAsync(sqlDelete, consolidacaoNotificacao);
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
