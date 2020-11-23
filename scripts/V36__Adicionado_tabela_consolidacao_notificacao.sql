CREATE TABLE if not exists public.consolidacao_notificacao (
	ano_letivo int2 NOT null,
	notificacao_id int8 NOT NULL,
	dre_codigo varchar(15) NOT NULL,
	ue_codigo varchar(15) NOT NULL,
	quantidade_responsaveis_com_app int8 NOT null default 0,
	quantidade_alunos_com_app int8 NOT null default 0,
	quantidade_responsaveis_sem_app int8 NOT null default 0,
	quantidade_alunos_sem_app int8 NOT null default 0
);


CREATE INDEX if not exists consolidacao_notificacao_ano_notificacao_dre_ue_idx ON public.consolidacao_notificacao (ano_letivo, notificacao_id, dre_codigo, ue_codigo);
