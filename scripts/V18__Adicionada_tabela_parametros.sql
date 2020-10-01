CREATE TABLE IF NOT EXISTS public.parametroescolaaqui (
	chave varchar(30) NOT NULL,
	conteudo varchar(100) NOT NULL DEFAULT ''
);

CREATE UNIQUE INDEX IF NOT EXISTS  parametroescolaaqui_chave_idx ON public.parametroescolaaqui (chave);