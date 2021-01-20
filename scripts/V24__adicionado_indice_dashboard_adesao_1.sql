 CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS 
	 dashboard_adesao_dre_ue_turma_idx 
ON 
	 public.dashboard_adesao (dre_codigo, ue_codigo, codigo_turma);