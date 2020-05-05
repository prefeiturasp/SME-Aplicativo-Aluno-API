 create table if not exists usuario_notificacao_leitura (
id bigint not null generated always as identity,
usuario_id bigint  not null,
codigoAlunoEol bigint not null,
notiicacao_id bigint not null,
criadoem timestamp);