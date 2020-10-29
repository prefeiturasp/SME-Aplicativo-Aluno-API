create index 
	frequencia_aluno_ue_turma_ano_bimestre_aluno_idx 
ON public.frequencia_aluno (
	ue_codigo,
	turma_codigo,
	ano_letivo,
	bimestre,
	aluno_codigo);