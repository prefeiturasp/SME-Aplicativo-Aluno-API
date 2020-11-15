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
    public class DadosLeituraRepositorio : BaseRepositorio<Adesao>, IDadosLeituraRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;

        public DadosLeituraRepositorio(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<DadosLeituraResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe)
        {
            try
            {
                var sql = @"select * from consolidacao_notificacao cn";

                using var conexao = InstanciarConexao();
                conexao.Open();
                var dadosLeituraComunicados = await conexao.QueryAsync<DadosLeituraResultado>(sql);
                conexao.Close();

                return dadosLeituraComunicados;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public Task<IEnumerable<DadosLeituraResultado>> ObterDadosLeituraComunicadosPorDre()
        {
            throw new NotImplementedException();
        }
   }
}