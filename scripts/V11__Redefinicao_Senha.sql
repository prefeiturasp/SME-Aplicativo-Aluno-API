ALTER TABLE public.usuario ADD if not exists token_redefinicao varchar(8) NULL;
ALTER TABLE public.usuario ADD if not exists redefinicao bool NOT NULL DEFAULT false;
ALTER TABLE public.usuario ADD if not exists validade_token timestamp NULL;
