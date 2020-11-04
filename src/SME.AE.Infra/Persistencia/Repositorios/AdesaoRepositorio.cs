using Dapper;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AdesaoRepositorio : BaseRepositorio<Adesao>, IAdesaoRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;

        public AdesaoRepositorio(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoAgrupadosPorDre()
        {
            try
            {
                var chaveCache = $"dadosAdesaoAgrupadosPorDre";
                var dashboardAdesao = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dashboardAdesao))
                    return JsonConvert.DeserializeObject<IEnumerable<TotaisAdesaoResultado>>(dashboardAdesao);

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosAdesaoAgrupadosPorDreUeETurma = await conexao.QueryAsync<TotaisAdesaoResultado>(AdesaoConsultas.ObterDadosAdesaoPorDre);
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosAdesaoAgrupadosPorDreUeETurma, 720, false);
                return dadosAdesaoAgrupadosPorDreUeETurma;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoSintetico(string codigoDre, string codigoUe)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoDre))
                    codigoDre = "";

                if (string.IsNullOrWhiteSpace(codigoUe))
                    codigoUe = "";

                var chaveCache = $"dadosAdesaoAgrupadosPorDreUeETurma-{codigoDre}-{codigoUe}";
                var dashboardAdesao = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dashboardAdesao))
                    return JsonConvert.DeserializeObject<IEnumerable<TotaisAdesaoResultado>>(dashboardAdesao);

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosAdesaoAgrupadosPorDreUeETurma = await conexao.QueryAsync<TotaisAdesaoResultado>(AdesaoConsultas.ObterDadosAdesao, new { dre_codigo = codigoDre, ue_codigo = codigoUe });
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosAdesaoAgrupadosPorDreUeETurma, 720, false);
                return dadosAdesaoAgrupadosPorDreUeETurma;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}