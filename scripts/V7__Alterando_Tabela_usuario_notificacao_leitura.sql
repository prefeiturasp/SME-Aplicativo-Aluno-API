alter table usuario_notificacao_leitura
add column if not exists dre_codigoEol bigint,
add column if not exists ue_codigoEol varchar (6),
add column if not exists usuario_cpf varchar (11),
add column if not exists alteradoEm  timestamp,
add column if not exists criadoPor    varchar(500),
add column if not exists alteradoPor  varchar(500),
add column if not exists mensagemVisualizada bool