 CREATE TABLE IF NOT EXISTS public.termos_de_uso (
  id bigint not null generated always as identity UNIQUE,
  descricao_termos_uso varchar,
  descricao_politica_privacidade varchar,
  versao varchar(12) not null,
  criado_em timestamp,
  criado_por varchar(100),
  alterado_em timestamp,
  alterado_por varchar(100)
);
