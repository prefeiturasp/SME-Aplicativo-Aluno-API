 CREATE TABLE public.termos_de_uso (
  id bigint not null generated always as identity,
  descricao_termos_uso nvarchar(),
  descricao_politica_privacidade nvarchar(),
  versao varchar(12) not null,
  ultimoLogin timestamp not null,
  criadoem timestamp
);
