using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class WorkerProcessoAtualizacaoRepositorio: IWorkerProcessoAtualizacaoRepositorio
	{
		private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

		public async Task IncluiOuAtualizaUltimaAtualizacao(string nomeProcesso)
		{

			var sqlUpdate = @"
				update worker_processo_atualizacao set data_ultima_atualizacao = @data_ultima_atualizacao where nome_processo = @nome_processo            
			";

			var sqlInsert = @"
				insert into worker_processo_atualizacao (data_ultima_atualizacao, nome_processo) values (@data_ultima_atualizacao, @nome_processo)
			";

			var parametros = new { nome_processo = nomeProcesso, data_ultima_atualizacao = DateTime.Now };

			using var conn = CriaConexao();
			await conn.OpenAsync();

			var alterado = (await conn.ExecuteAsync(sqlUpdate, parametros)) > 0;
			if (!alterado)
			{
				await conn.ExecuteAsync(sqlInsert, parametros);
			}

			await conn.CloseAsync();
		}
	}
}
