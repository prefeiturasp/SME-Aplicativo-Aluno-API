using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class WorkerProcessoAtualizacaoRepositorio : IWorkerProcessoAtualizacaoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public WorkerProcessoAtualizacaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

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

        public async Task<UltimaAtualizaoWorkerPorProcessoResultado> ObterUltimaAtualizacaoPorProcesso(string nomeProcesso)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();
                var ultimaAtualizacao = await conexao.QueryFirstAsync<UltimaAtualizaoWorkerPorProcessoResultado>($"select nome_processo as NomeProcesso, data_ultima_atualizacao as DataUltimaAtualizacao from worker_processo_atualizacao wpa where nome_processo = @nomeProcesso", new { nomeProcesso });
                conexao.Close();
                return ultimaAtualizacao;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
