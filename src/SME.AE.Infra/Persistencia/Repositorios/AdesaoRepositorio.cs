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

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoAgrupadosPorDreUeETurma(string codigoDre, string codigoUe)
        {
            try
            {
                var chaveCache = $"dadosAdesaoAgrupadosPorDreUeETurma-{codigoDre}-{codigoUe}";
                var dashboardAdesao = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dashboardAdesao))
                    return JsonConvert.DeserializeObject<IEnumerable<TotaisAdesaoResultado>>(dashboardAdesao);

                var query = new StringBuilder();
                query.AppendLine($"{AdesaoConsultas.ObterDadosAdesaoAgrupadosPorDreUeETurma}");

                if (!string.IsNullOrEmpty(codigoDre))
                    query.AppendLine($" AND dre_codigo = '{codigoDre}'");

                if (!string.IsNullOrEmpty(codigoUe))
                    query.AppendLine($" AND ue_codigo = '{codigoUe}'");

                query.AppendLine($" GROUP BY dre_codigo, dre_nome, ue_codigo, ue_nome, codigo_turma ");

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosAdesaoAgrupadosPorDreUeETurma = await conexao.QueryAsync<TotaisAdesaoResultado>(query.ToString(), new { codigoDre, codigoUe });
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

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoSme()
        {
            try
            {
                var chaveCache = $"dadosAdesaoSme";
                var dashboardAdesao = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dashboardAdesao))
                    return JsonConvert.DeserializeObject<IEnumerable<TotaisAdesaoResultado>>(dashboardAdesao);

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosAdesaoSme = await conexao.QueryAsync<TotaisAdesaoResultado>(AdesaoConsultas.ObterDadosAdesaoSme);
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosAdesaoSme, 720, false);
                return dadosAdesaoSme;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}