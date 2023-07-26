using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TermosDeUsoRepositorio : BaseRepositorio<TermosDeUso>, ITermosDeUsoRepositorio
    {
        private readonly IServicoTelemetria servicoTelemetria;

        public TermosDeUsoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<TermosDeUso> ObterTermosDeUsoPorCpf(string cpf)
        {
            try
            {
                using var conexao = InstanciarConexao();

                conexao.Open();

                var parametros = new { cpf };

                var termosDeUso = await servicoTelemetria.RegistrarComRetornoAsync<TermosDeUso>(async () =>
                    await SqlMapper.QueryFirstAsync<TermosDeUso>(conexao, TermosDeUsoConsultas.ObterTermosDeUsoPorCpf, parametros), "query", "Query AE",
                        TermosDeUsoConsultas.ObterTermosDeUsoPorCpf, parametros.ToString());

                conexao.Close();

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

        public async Task<TermosDeUso> ObterPorId(long id)
        {
            try
            {
                using var conexao = InstanciarConexao();

                conexao.Open();

                var sql = $"{TermosDeUsoConsultas.ObterTermosDeUso} WHERE Id = @TermoDeUsoId";
                var parametros = new { id };

                var termosDeUso = await servicoTelemetria.RegistrarComRetornoAsync<TermosDeUso>(async () =>
                    await SqlMapper.QueryFirstAsync<TermosDeUso>(conexao, sql, parametros), "query", "Query AE", sql, parametros.ToString());

                conexao.Close();

                return termosDeUso;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task<TermosDeUso> ObterUltimaVersaoTermosDeUso()
        {
            try
            {
                using var conexao = InstanciarConexao();

                conexao.Open();

                var termosDeUso = await servicoTelemetria.RegistrarComRetornoAsync<TermosDeUso>(async () =>
                    await SqlMapper.QueryFirstAsync<TermosDeUso>(conexao, TermosDeUsoConsultas.ObterUltimaVersaoDosTermosDeUso),
                        "query", "Query AE", TermosDeUsoConsultas.ObterUltimaVersaoDosTermosDeUso);

                conexao.Close();

                return termosDeUso;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }
    }
}