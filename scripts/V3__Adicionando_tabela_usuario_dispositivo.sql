 create table if not exists usuario_dispositivo (
id bigint not null generated always as identity,
usuario_id bigint not null ,
codigo_dispositivo varchar(200) not null ,
criadoem timestamp);
