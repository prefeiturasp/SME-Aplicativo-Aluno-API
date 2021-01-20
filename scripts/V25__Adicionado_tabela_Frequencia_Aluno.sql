CREATE TABLE IF NOT EXISTS public.frequencia_aluno (
	ue_codigo char(6) NOT NULL,
	ue_nome varchar(100) NOT NULL,
	turma_codigo bigint NOT NULL,
	turma_descricao varchar(100) NOT NULL,
	aluno_codigo varchar(8) NOT NULL,
	bimestre int NOT NULL,
	componente_curricular varchar(100) NOT NULL,
	quantidade_aulas bigint NOT NULL,
	quantidade_faltas bigint NOT NULL,
	quantidade_compensacoes bigint NOT NULL
);
