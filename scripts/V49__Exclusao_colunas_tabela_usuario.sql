ALTER TABLE public.usuario 
DROP COLUMN IF EXISTS nome,
DROP COLUMN IF EXISTS email,
DROP COLUMN IF EXISTS celular;