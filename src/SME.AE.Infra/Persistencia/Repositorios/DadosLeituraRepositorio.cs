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

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe, long notificaoId, int modoVisualizacao)
        {
            try
            {
                var sql = "";
                if (string.IsNullOrEmpty(codigoDre) && string.IsNullOrEmpty(codigoUe))
                    sql = @"select * from consolidacao_notificacao cn where ano_letivo = 2020 and dre_codigo = '' and ue_codigo = '' ";

                if (string.IsNullOrEmpty(codigoDre) && string.IsNullOrEmpty(codigoUe))
                    sql = @"select * from consolidacao_notificacao cn where ano_letivo = 2020 and dre_codigo <> '' and ue_codigo = '' ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosLeituraComunicadosResultado>(sql);
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> ObterDadosLeituraComunicadosPorDre(long notificaoId, int modoVisualizacao)
        {
            try
            {
                var sql = @"select * from consolidacao_notificacao cn where ano_letivo = 2020 and dre_codigo <> '' and ue_codigo = '' ";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosLeituraComunicadosResultado>(sql);
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