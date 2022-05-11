using Dapper;
using Npgsql;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra
{
    public class OutroServicoRepositorio : IOutroServicoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public OutroServicoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<IEnumerable<OutroServicoDto>> Links()
        {
            var sql = @"select 
	                        titulo,
	                        descricao,
	                        categoria,
	                        urlsite,
	                        icone,
	                        destaque
                        from OutroServico
                        where ativo
                        order by categoria,titulo";

            try
            {
                using var conexao = CriaConexao();

                conexao.Open();

                var lista = await servicoTelemetria.RegistrarComRetornoAsync<OutroServicoDto>(async () =>
                    await SqlMapper.QueryAsync<OutroServicoDto>(conexao, sql), "query", "Query AE", sql);

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
            var sql = @"select 
	                        titulo,
	                        descricao,
	                        categoria,
	                        urlsite,
	                        icone,
	                        destaque,ordem
                        from OutroServico
                        where ativo and destaque
                        order by ordem limit 6;";
            try
            {
                using var conexao = CriaConexao();

                conexao.Open();

                var lista = await servicoTelemetria.RegistrarComRetornoAsync<OutroServicoDto>(async () =>
                    await SqlMapper.QueryAsync<OutroServicoDto>(conexao, sql), "query", "Query AE", sql);

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
