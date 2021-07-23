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
    public class EventoSgpRepositorio : IEventoSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public EventoSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<IEnumerable<EventoSgpDto>> ListaEventoPorDataAlteracao(DateTime ultimaDataAlteracao)
        {
            var sql =
                @"
					select * from 
					(
						(select 
							coalesce (cc.descricao_sgp, cc.descricao) as componente_curricular,							
							aa.id, aa.nome_avaliacao nome, aa.descricao_avaliacao descricao, aa.data_avaliacao data_inicio, aa.data_avaliacao data_fim,
							aa.dre_id, aa.ue_id, 
							0::int tipo_evento_id,
							aa.turma_id,
							t.ano_letivo,
							t.modalidade_codigo modalidade_turma,
							null::int modalidade_calendario,
							greatest(aa.criado_em, aa.alterado_em) alterado_em,
							coalesce(aa.excluido, false) excluido,
							null::int tipo_calendario_id
						from atividade_avaliativa aa 
						inner join turma t on t.turma_id = aa.turma_id
						inner join atividade_avaliativa_disciplina aad on aad.atividade_avaliativa_id = aa.id
						inner join componente_curricular cc on cc.id::varchar = aad.disciplina_id 
						where
							(aa.migrado isnull or aa.migrado = false) 
						)
						union 
						(select
							null as componente_curricular,
							e.id, e.nome, e.descricao, e.data_inicio, e.data_fim,
							e.dre_id, e.ue_id,
							e.tipo_evento_id,
							null::varchar turma_id,
							tc.ano_letivo,
							null::int modalidade_turma,
							tc.modalidade modalidade_calendario,
							greatest(
								e.criado_em, e.alterado_em,
								tc.criado_em, tc.alterado_em,
								wan.criado_em, wan.alterado_em,
								n2.criado_em, n2.alterado_em) alterado_em,
							coalesce(e.excluido, false) excluido,
							tc.id tipo_calendario_id
						from evento e
						inner join evento_tipo et on et.id = e.tipo_evento_id
						inner join tipo_calendario tc on tc.id = e.tipo_calendario_id 
						left join wf_aprovacao_nivel wan on wan.wf_aprovacao_id = e.wf_aprovacao_id 
						left join wf_aprovacao_nivel_notificacao wann on wann.wf_aprovacao_nivel_id = wan.id 
						left join notificacao n2 on n2.id = wann.notificacao_id 
						where
							(e.status = 1) and
							(e.migrado isnull or e.migrado = false) and
							(et.evento_escolaaqui)
						)
					) as eae
					where alterado_em >= @alterado_em 
					order by alterado_em
				";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var eventosSgp = await conn.QueryAsync<EventoSgpDto>(sql, new { alterado_em = ultimaDataAlteracao });
            await conn.CloseAsync();
            return eventosSgp;
        }
    }
}
