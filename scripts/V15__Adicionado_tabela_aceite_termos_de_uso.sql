CREATE TABLE IF NOT EXISTS  public.aceite_termos_de_uso (
  id bigint not null generated always as identity UNIQUE,
  termos_de_uso_id bigint not null,
  usuario varchar(100) not null,
  device varchar(50) not null,
  ip varchar(12) not null,
  versao decimal not null,
  data_hora_aceite timestamp not null,
  hash varchar not null,
  criado_em timestamp,
  criado_por varchar(100),
  alterado_em timestamp,
  alterado_por varchar(100)
);


