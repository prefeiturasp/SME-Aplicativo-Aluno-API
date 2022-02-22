﻿using Dapper;
using Npgsql;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra
{
    public class OutroServicoRepositorio : IOutroServicoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        public OutroServicoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
        public async Task<IEnumerable<OutroServicoDto>> Links()
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();
                var lista = await conexao.QueryAsync<OutroServicoDto>(@"
                        select 
	                        titulo,
	                        descricao,
	                        categoria,
	                        urlsite,
	                        icone,
	                        destaque
                        from OutroServico
                        where ativo ");
                conexao.Close();

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OutroServicoDto>> LinksPrioritarios()
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();
                var lista = await conexao.QueryAsync<OutroServicoDto>(@"
                        select 
	                        titulo,
	                        descricao,
	                        categoria,
	                        urlsite,
	                        icone,
	                        destaque
                        from OutroServico
                        where ativo and destaque");
                conexao.Close();

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
