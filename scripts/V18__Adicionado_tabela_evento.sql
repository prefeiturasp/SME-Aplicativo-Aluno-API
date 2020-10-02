CREATE TABLE IF NOT exists evento (
	evento_id varchar(50) NOT NULL,
	nome varchar(200) NOT NULL,
	descricao varchar(5000) NULL,
	data_inicio timestamp NOT NULL,
	data_fim timestamp NOT NULL,
	dre_id varchar(50) NULL,
	ue_id varchar(50) NULL,
	tipo_evento int4 NOT NULL,
	turma_id varchar(50) NULL,
	ano_letivo int4 NOT NULL,
	modalidade int4 NOT NULL,
	ultima_alteracao_sgp timestamp NOT NULL,
	CONSTRAINT evento_pk PRIMARY KEY (evento_id)
);
CREATE INDEX IF NOT exists evento_ultima_alteracao_sgp_idx ON public.evento (ultima_alteracao_sgp);