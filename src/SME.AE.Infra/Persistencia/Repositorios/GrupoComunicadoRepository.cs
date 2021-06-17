using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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

        public GrupoComunicadoRepository(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        public async Task<IEnumerable<GrupoComunicado>> ObterPorIds(string ids)
        {
            using var conexao = new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);
            conexao.Open();
            var resultado = await conexao.QueryAsync<GrupoComunicado>(
                 GrupoComunicadoConsulta.Select +
                 @"WHERE id in(" + ids + ")");
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
                    resultado = await conexao.QueryAsync<GrupoComunicado>(GrupoComunicadoConsulta.Select);
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
