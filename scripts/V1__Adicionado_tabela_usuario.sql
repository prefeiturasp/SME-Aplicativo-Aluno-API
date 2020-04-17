 CREATE TABLE Usuario (
  id bigint not null generated always as identity,
  cpf varchar(12) not null,
  nome varchar(256),
  ultimoLogin timestamp not null,
  email varchar(128),
  criadoem timestamp,
  excluido bool
);
