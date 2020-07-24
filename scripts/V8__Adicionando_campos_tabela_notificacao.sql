alter table notificacao

add column if not exists dre_codigoEol varchar(6),
add column if not exists ue_codigoEol varchar (6),
add column if not exists ano_letivo int,
add column if not exists tipoComunicado int;