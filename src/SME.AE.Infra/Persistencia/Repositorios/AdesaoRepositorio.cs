using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AdesaoRepositorio : IAdesaoRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;

        public AdesaoRepositorio(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterTotaisAdesao(string codigoDre, string codigoUe)
        {
            try
            {
                //var chaveCache = $"dadosAlunos-{cpf}";
                //var dadosAlunos = await cacheRepositorio.ObterAsync(chaveCache);
                //if (!string.IsNullOrWhiteSpace(dadosAlunos))
                //    return JsonConvert.DeserializeObject<List<AlunoRespostaEol>>(dadosAlunos);

                using var conexao = new SqlConnection(ConnectionStrings.Conexao);
                var adesao = await conexao.QueryAsync<TotaisAdesaoResultado>($"select * from adesao", new { codigoDre, codigoUe });
                return (List<TotaisAdesaoResultado>)adesao;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }


        public Task<IEnumerable<Adesao>> ListarAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Adesao> ObterPorIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task RemoverAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task RemoverAsync(Adesao entidade)
        {
            throw new NotImplementedException();
        }

        public Task<long> SalvarAsync(Adesao entidade)
        {
            throw new NotImplementedException();
        }

    }
}