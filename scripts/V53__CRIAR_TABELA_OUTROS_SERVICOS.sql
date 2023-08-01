
CREATE TABLE public.OutroServico (
	id int8 NOT NULL GENERATED ALWAYS AS IDENTITY,
	titulo varchar(40) NOT NULL,
	descricao varchar(100) NOT NULL,
	categoria varchar(30) NOT NULL,
	urlsite varchar(200) NOT NULL,
	icone varchar(200) NOT NULL,
	destaque bool default false,
	ativo bool default  true,
	ordem int,
	criado_em timestamp NOT NULL,
	criado_por varchar(200) NOT NULL,
	alterado_em timestamp NULL,
	alterado_por varchar(200) NULL,
	criado_rf varchar(200) NOT NULL,
	alterado_rf varchar(200) NULL
);
