using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ConsolidarLeituraNotificacaoRepositorio: IConsolidarLeituraNotificacaoRepositorio
	{
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

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
                //SentrySdk.CaptureException(ex);
                throw ex;
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
                    AND ano_letivo=@AnoLetivo
                ";

            const string sqlUpdate =
                @"
                update consolidacao_notificacao
                set
	                quantidade_alunos = @QuantidadeAlunos,
	                quantidade_responsaveis = @QuantidadeResponsaveis 
                where 
	                notificacao_id=@NotificacaoId 
                and dre_codigo=@DreCodigo
                and ue_codigo=@UeCodigo
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
	                quantidade_responsaveis,
	                quantidade_alunos
                ) values (
                    @AnoLetivo,
	                @NotificacaoId,
	                @DreCodigo,
	                @UeCodigo,
	                @QuantidadeResponsaveis,
	                @QuantidadeAlunos
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
                //SentrySdk.CaptureException(ex);
                throw ex;
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
                //SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
