CREATE TABLE IF NOT EXISTS public.worker_processo_atualizacao (
	id bigint not null generated always as identity,
	nome_processo varchar(100) NOT NULL DEFAULT '',
	data_ultima_atualizacao timestamp not null
);

CREATE UNIQUE INDEX IF NOT EXISTS  worker_processo_atualizacao_id_idx ON public.worker_processo_atualizacao (id);