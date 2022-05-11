using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ConsolidarLeituraNotificacaoSgpRepositorio : IConsolidarLeituraNotificacaoSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public ConsolidarLeituraNotificacaoSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<IEnumerable<ComunicadoSgpDto>> ObterComunicadosSgp()
        {
            const string sql =
                @"
                select
	                c.id Id,
	                c.ano_letivo AnoLetivo,
	                c.codigo_dre CodigoDre,
	                c.codigo_ue CodigoUe,
	                ct.turma_codigo TurmaCodigo,
	                ca.aluno_codigo AlunoCodigo,
	                c.modalidade modalidade,
	                c.series_resumidas SeriesResumidas,
	                c.tipo_comunicado TipoComunicado,
	                gc.tipo_escola_id TipoEscolaId,
	                gc.etapa_ensino_id EtapaEnsinoId,
	                gc.tipo_ciclo_id TipoCicloId
                from 
	                comunicado c
                left join comunicado_aluno ca on ca.comunicado_id = c.id 
                left join comunicado_turma ct on ct.comunicado_id = c.id
                left join comunidado_grupo cg on cg.comunicado_id = c.id
                left join grupo_comunicado gc on cg.grupo_comunicado_id = gc.id 
                where 
	               c.ano_letivo >= extract(year from current_date)
                --and	date_trunc('day', c.data_envio) <= current_date 
                --and date_trunc('day', c.data_expiracao) >= current_date
                and not c.excluido  and c.tipo_comunicado <> @tipocomunicado
                and not coalesce(ca.excluido, false)
                and not coalesce(ct.excluido, false)
                and not coalesce(cg.excluido, false)
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();

                var parametros = new { tipocomunicado = TipoComunicado.MENSAGEM_AUTOMATICA };
                var comunicadosSgp = await servicoTelemetria.RegistrarComRetornoAsync<ComunicadoSgpDto>(async () => await SqlMapper.QueryAsync<ComunicadoSgpDto>(conn, sql, parametros),
                    "query", "Query SGP", sql, parametros.ToString());

                conn.Close();
                return comunicadosSgp;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
