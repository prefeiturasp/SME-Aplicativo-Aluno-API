ALTER TABLE if exists public.notificacao ADD if not exists excluido boolean not null default false;
ALTER TABLE if exists public.notificacao_aluno ADD if not exists excluido boolean not null default false;
ALTER TABLE if exists public.notificacao_turma ADD if not exists excluido boolean not null default false;
ALTER TABLE if exists public.usuario_notificacao_leitura ADD if not exists excluido boolean not null default false;