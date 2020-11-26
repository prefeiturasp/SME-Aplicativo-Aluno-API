drop index if exists consolidacao_notificacao_ano_notificacao_dre_ue_idx;

alter table if exists consolidacao_notificacao
add column if not exists turma varchar(10) not null default '';

alter table if exists consolidacao_notificacao
add column if not exists turma_codigo int8 not null default 0;

alter table if exists consolidacao_notificacao
add column if not exists modalidade_codigo int2 not null default 0;

CREATE INDEX if not exists consolidacao_notificacao_ano_notificacao_dre_ue_mod_turma_idx ON public.consolidacao_notificacao (ano_letivo, notificacao_id, dre_codigo, ue_codigo, modalidade_codigo, turma_codigo);
