ALTER TABLE public.usuario ADD if not exists token_redefinicao varchar(8) NULL;
ALTER TABLE public.usuario ADD if not exists redefinicao bool NOT NULL DEFAULT false;
ALTER TABLE public.usuario ADD if not exists validade_token timestamp NULL;

CREATE TABLE if not exists public.configuracao_email (
	id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
	email_remetente varchar(100) NOT NULL,
	nome_remetente varchar(100) NOT NULL,
	porta int NOT NULL,
	senha varchar(200) NOT NULL,
	servidor_smtp varchar(200) NOT NULL,
	tls bool NOT NULL,
	usuario varchar(200) NOT NULL
);

