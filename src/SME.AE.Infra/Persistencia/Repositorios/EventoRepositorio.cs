using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class EventoRepositorio : IEventoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public EventoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<DateTime?> ObterUltimaAlteracao()
        {
            var sql = @"select max(ultima_alteracao_sgp) from evento";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            var ultimaAlteracao = await conn.QueryFirstOrDefaultAsync<DateTime?>(sql);
            await conn.CloseAsync();
            return ultimaAlteracao;
        }
        public async Task Remover(EventoDto evento)
        {
            var sql = @"
                delete from evento
                where evento_id = @evento_id
            ";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            await conn.ExecuteAsync(sql, evento);
            await conn.CloseAsync();
            return;
        }

        public async Task Salvar(EventoDto evento)
        {
            var sql = @"
                insert into evento
	                (evento_id, nome, descricao, data_inicio, data_fim, dre_id, ue_id, tipo_evento, turma_id, ano_letivo, modalidade, ultima_alteracao_sgp, componente_curricular, tipo_calendario_id)
                values
	                (@evento_id, @nome, @descricao, @data_inicio, @data_fim, @dre_id, @ue_id, @tipo_evento, @turma_id, @ano_letivo, @modalidade, @ultima_alteracao_sgp, @componente_curricular, @tipo_calendario_id)
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
	                ultima_alteracao_sgp = excluded.ultima_alteracao_sgp,
                    componente_curricular = excluded.componente_curricular,
                    tipo_calendario_id = excluded.tipo_calendario_id
            ";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            await conn.ExecuteAsync(sql, evento);
            await conn.CloseAsync();
            return;
        }

        public async Task<IEnumerable<EventoDto>> ObterPorDreUeTurmaMes(string dre_id, string ue_id, string turma_id, int modalidadeCalendario, DateTime mesAno)
        {
            var dataInicio = new DateTime(mesAno.Year, mesAno.Month, 1);
            var dataFim = dataInicio.AddMonths(1).AddMilliseconds(-1);
            var queryDre = string.IsNullOrWhiteSpace(dre_id) ? "" : "and (dre_id isnull or dre_id = @dre_id)";
            var queryUe = string.IsNullOrWhiteSpace(ue_id) ? "" : "and (ue_id isnull or ue_id = @ue_id)";
            var queryTurmna = string.IsNullOrWhiteSpace(turma_id) ? "" : "and (turma_id isnull or turma_id = @turma_id)";

            var sql = @$"
                Select distinct
	                evento_id, nome, descricao, data_inicio, data_fim, dre_id, ue_id, tipo_evento, turma_id, ano_letivo, modalidade, ultima_alteracao_sgp, componente_curricular, tipo_calendario_id
                from
	                evento
                where
                    (data_inicio between @dataInicio and @dataFim or data_fim between @dataInicio and @dataFim)
                    and modalidade = @modalidadeCalendario
                    {queryDre}
                    {queryUe}
                    {queryTurmna}
            ";
            using var conn = CriaConexao();
            await conn.OpenAsync();
            var eventos = await conn.QueryAsync<EventoDto>(sql, new { dataInicio, dataFim, dre_id, ue_id, turma_id, modalidadeCalendario });
            await conn.CloseAsync();
            return eventos;
        }
    }
}
