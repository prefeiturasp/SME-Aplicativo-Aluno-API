alter table if exists dashboard_adesao
alter column dre_codigo set not null,
alter column dre_codigo drop default,
alter column ue_codigo set not null,
alter column ue_codigo drop default,
alter column codigo_turma set not null,
alter column codigo_turma drop default;

CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS 
	dashboard_adesao_dre_ue_turma_idx 
ON 
	public.dashboard_adesao (dre_codigo, ue_codigo, codigo_turma);