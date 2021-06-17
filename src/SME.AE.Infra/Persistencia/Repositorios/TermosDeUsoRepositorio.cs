using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TermosDeUsoRepositorio : BaseRepositorio<TermosDeUso>, ITermosDeUsoRepositorio
    {
        public TermosDeUsoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions) : base(variaveisGlobaisOptions.AEConnection)
        {
        }

        public async Task<TermosDeUso> ObterTermosDeUsoPorCpf(string cpf)
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>($"{TermosDeUsoConsultas.ObterTermosDeUsoPorCpf}", new { cpf });
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
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>($"{TermosDeUsoConsultas.ObterTermosDeUso} WHERE Id = @TermoDeUsoId", new { id });
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
                var termosDeUso = await conexao.QueryFirstAsync<TermosDeUso>(TermosDeUsoConsultas.ObterUltimaVersaoDosTermosDeUso);
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