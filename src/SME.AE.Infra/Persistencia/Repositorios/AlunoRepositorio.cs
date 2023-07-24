using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly IServicoTelemetria servicoTelemetria;
        public AlunoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions, IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
        }

        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
    
        public async Task<List<CpfResponsavelAlunoEol>> ObterCpfsDeResponsaveis(string codigoDre, string codigoUe)
        {
            try
            {
                var query = new StringBuilder();
                query.AppendLine($"{AlunoConsultas.ObterCpfsResponsaveis}");

                if (!string.IsNullOrWhiteSpace(codigoDre))
                    query.AppendLine(" AND dre.cd_unidade_educacao = @codigoDre ");

                if (!string.IsNullOrWhiteSpace(codigoUe))
                    query.AppendLine(" AND vue.cd_unidade_educacao = @codigoUe");

                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                var cpfsResponsaveis = await conexao.QueryAsync<CpfResponsavelAlunoEol>($"{query}", new { codigoDre, codigoUe });
                return cpfsResponsaveis.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}