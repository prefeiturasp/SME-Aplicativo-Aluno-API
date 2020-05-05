 create table if not exists usuario_notificacao_leitura (
id bigint not null generated always as identity,
usuario_id bigint  not null,
codigo_eol_aluno bigint not null,
notificacao_id bigint not null,
criadoem timestamp);