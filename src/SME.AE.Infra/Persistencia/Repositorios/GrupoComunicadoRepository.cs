using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class GrupoComunicadoRepository : IGrupoComunicadoRepository
    {
        public async Task<IEnumerable<GrupoComunicado>> ObterPorIds(string ids)
        {
            using var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp);
            conexao.Open();
            var resultado = await conexao.QueryAsync<GrupoComunicado>(
                 GrupoComunicadoConsulta.Select + 
                 @"WHERE id in(" + ids + ")");
            conexao.Close();
            return resultado;
        }
        
        public async Task<IEnumerable<GrupoComunicado>> ObterTodos()
        {
            using var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp);
            conexao.Open();
            var resultado = await conexao.QueryAsync<GrupoComunicado>(GrupoComunicadoConsulta.Select);
            conexao.Close();
            return resultado;
        }
    }
}
