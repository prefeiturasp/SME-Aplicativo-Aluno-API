using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class DreSgpRepositorio : IDreSgpRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

        public DreSgpRepositorio(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        private static string chaveCacheDre(string codigoDre)
            => $"Dre-NomeAbreviado-{codigoDre}";

        public async Task<DreResposta> ObterNomeAbreviadoDrePorCodigo(string codigoDre)
        {
            try
            {
                var chaveCache = chaveCacheDre(codigoDre);
                var dre = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dre))
                    return JsonConvert.DeserializeObject<DreResposta>(dre);

                using var conexao = CriaConexao();
                conexao.Open();
                var query = @"SELECT abreviacao as NomeAbreviado FROM dre WHERE dre_id = @codigoDre ";
                var nomeAbreviadoDre = await conexao.QuerySingleAsync<DreResposta>(query, new { codigoDre });
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, nomeAbreviadoDre, 720, false);
                return nomeAbreviadoDre;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<long>> ObterTodosCodigoDresAtivasAsync()
        {
            using var conexao = CriaConexao();

            conexao.Open();

            var query = @"SELECT dre_id FROM dre";

            var ids = await conexao.QueryAsync<long>(query);

            conexao.Close();

            return ids;
        }

        public async IAsyncEnumerator<long> ObterTodosCodigosDresAtivasStream()
        {
            using var conexao = CriaConexao();
            await conexao.OpenAsync();
            
            var query = @"SELECT dre_id FROM dre";
            var ids = await conexao.QueryAsync<long>(query);
            var enumerador = ids.GetEnumerator();

            while (enumerador.MoveNext())
            {
                yield return enumerador.Current;
            }

            await conexao.CloseAsync();
        }
    }
}