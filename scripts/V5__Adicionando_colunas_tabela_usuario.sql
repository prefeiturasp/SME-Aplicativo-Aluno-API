ALTER table if exists public.usuario add if not exists celular varchar(20) NULL;
ALTER TABLE if exists public.usuario ADD if not exists primeiroacesso bool NOT NULL DEFAULT false;
ALTER TABLE if exists public.usuario ADD if not exists alteradoem timestamp NULL;
ALTER TABLE if exists public.usuario ADD if not exists criadopor varchar(200) NOT NULL DEFAULT 'Sistema';
ALTER TABLE if exists public.usuario ADD if not exists alteradopor varchar(200) NULL;