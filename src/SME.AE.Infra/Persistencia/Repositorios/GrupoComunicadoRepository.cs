using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class GrupoComunicadoRepository : IGrupoComunicadoRepository
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public GrupoComunicadoRepository(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<IEnumerable<GrupoComunicado>> ObterPorIds(string ids)
        {
            var sql = $"{GrupoComunicadoConsulta.Select} WHERE id in({ids})";

            using var conexao = new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

            conexao.Open();

            var resultado = await servicoTelemetria.RegistrarComRetornoAsync<GrupoComunicado>(async () => await SqlMapper.QueryAsync<GrupoComunicado>(conexao, sql),
                "query", "Query SGP", sql);

            conexao.Close();

            return resultado;
        }

        public async Task<IEnumerable<GrupoComunicado>> ObterTodos()
        {
            IEnumerable<GrupoComunicado> resultado = null;

            using (var conexao = new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection))
            {
                try
                {
                    conexao.Open();

                    resultado = await servicoTelemetria.RegistrarComRetornoAsync<GrupoComunicado>(async () => await SqlMapper.QueryAsync<GrupoComunicado>(conexao, GrupoComunicadoConsulta.Select),
                        "query", "Query SGP", GrupoComunicadoConsulta.Select);

                    conexao.Close();
                }
                catch (Exception)
                {
                    conexao.Close();
                }
            }

            return resultado;
        }
    }
}
