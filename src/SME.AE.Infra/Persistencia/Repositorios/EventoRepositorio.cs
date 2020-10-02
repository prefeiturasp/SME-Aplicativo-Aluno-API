using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class EventoRepositorio : IEventoRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

        public async Task<DateTime?> ObterUltimaAlteracao()
        {
            var sql = @"select max(ultima_alteracao_sgp) from evento";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            var ultimaAlteracao = await conn.QueryFirstOrDefaultAsync<DateTime?>(sql);
            await conn.CloseAsync();
            return ultimaAlteracao;
        }

        public async Task Salvar(EventoDto evento)
        {
            var sql = @"
                insert into evento
	                (evento_id, nome, descricao, data_inicio, data_fim, dre_id, ue_id, tipo_evento, turma_id, ano_letivo, modalidade, ultima_alteracao_sgp)
                values
	                (@evento_id, @nome, @descricao, @data_inicio, @data_fim, @dre_id, @ue_id, @tipo_evento, @turma_id, @ano_letivo, @modalidade, @ultima_alteracao_sgp)
                on conflict 
	                (evento_id) 
                do update set 
	                nome = excluded.nome, 
	                descricao = excluded.descricao, 
	                data_inicio = excluded.data_inicio, 
	                data_fim = excluded.data_fim, 
	                dre_id = excluded.dre_id, 
	                ue_id = excluded.ue_id, 
	                tipo_evento = excluded.tipo_evento, 
	                turma_id = excluded.turma_id, 
	                ano_letivo = excluded.ano_letivo, 
	                modalidade = excluded.modalidade, 
	                ultima_alteracao_sgp = excluded.ultima_alteracao_sgp
            ";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            await conn.ExecuteAsync(sql, evento);
            await conn.CloseAsync();
            return;
        }
    }
}
