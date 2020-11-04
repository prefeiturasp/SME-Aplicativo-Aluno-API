CREATE TABLE if not exists nota_aluno (
	ano_letivo int2 NOT NULL,
	ue_codigo varchar(6) NOT NULL,
	turma_codigo varchar(8) NOT NULL,
	bimestre int4 NOT NULL,
	aluno_codigo varchar(8) NOT NULL,
	componente_curricular_codigo int8 NOT NULL,
	componente_curricular varchar(100),
	Nota varchar(8)
);
CREATE INDEX if not exists nota_aluno_ano_ue_turma_bimestre_aluno_diciplina_idx 
ON public.nota_aluno (ano_letivo, ue_codigo, turma_codigo, bimestre, aluno_codigo, componente_curricular_codigo);