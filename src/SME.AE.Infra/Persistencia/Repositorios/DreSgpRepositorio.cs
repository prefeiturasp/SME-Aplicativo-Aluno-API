﻿using Dapper;
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
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

        public async Task<DreResposta> ObterNomeAbreviadoDrePorCodigo(string codigoDre)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();
                var query = @"SELECT abreviacao as NomeAbreviado FROM dre WHERE dre_id = @codigoDre ";
                var nomeAbreviadoDre = await conexao.QuerySingleAsync<DreResposta>(query, new { codigoDre });
                conexao.Close();

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
    }
}