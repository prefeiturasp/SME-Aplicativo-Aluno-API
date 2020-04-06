CREATE TABLE public.Usuario(
	Id bigint not null generated always as identity,
	Cpf varchar(12) not null,
	Nome varchar(256),
	UltimoLogin timestamp not null
);
