CREATE TABLE IF NOT EXISTS public.dashboard_adesao (
	id bigint not null generated always as identity,
	dre_codigo char(6) NOT NULL DEFAULT '',
	dre_nome varchar(100) NOT NULL DEFAULT '',
	ue_codigo char(6) NOT NULL DEFAULT '',
	ue_nome varchar(100) NOT NULL DEFAULT '',
	codigo_turma bigint not null,
	usuarios_primeiro_acesso_incompleto bigint not null,
	usuarios_validos bigint not null,
	usuarios_cpf_invalidos bigint not null,
	usuarios_sem_app_instalado bigint not null
);

CREATE UNIQUE INDEX IF NOT EXISTS  dashboard_adesao_id_idx ON public.dashboard_adesao (id);