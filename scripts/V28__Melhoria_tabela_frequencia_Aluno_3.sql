create index 
	frequencia_aluno_ue_turma_ano_bimestre_aluno_idx 
ON public.frequencia_aluno (
	ano_letivo,
	bimestre,
	ue_codigo,
	turma_codigo,
	aluno_codigo);