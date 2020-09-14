using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TermosDeUsoRepositorio : BaseRepositorio<TermosDeUso>, ITermosDeUsoRepositorio
    {

        private const string ULTIMAVERSAOTERMOSDEUSO = "ultimaVersaoTermosDeUso";
        private readonly ICacheRepositorio cacheRepositorio;

        public TermosDeUsoRepositorio(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<TermosDeUso> ObterUltimaVersao()
        {
            try
            {
                var ultimaVersaoTermosDeUsoCache = ObterUltimaVersaoCache();
                if (ultimaVersaoTermosDeUsoCache != null)
                    return ultimaVersaoTermosDeUsoCache;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>(TermosDeUsoConsultas.ObterUltimaVersaoDosTermosDeUso);
                conexao.Close();

                var chaveUltimaVersaoTermosDeUso = $"{ULTIMAVERSAOTERMOSDEUSO}";
                await cacheRepositorio.SalvarAsync(chaveUltimaVersaoTermosDeUso, termosDeUso);
                return termosDeUso;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        async Task<long> ITermosDeUsoRepositorio.SalvarAsync(TermosDeUso termos)
        {
            return await base.SalvarAsync(termos);
        }

        private TermosDeUso ObterUltimaVersaoCache()
    => cacheRepositorio.Obter<TermosDeUso>($"{ULTIMAVERSAOTERMOSDEUSO}");

    }
}