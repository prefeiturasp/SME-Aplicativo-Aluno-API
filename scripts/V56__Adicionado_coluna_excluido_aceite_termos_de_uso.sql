ALTER TABLE public.aceite_termos_de_uso 
ADD COLUMN IF NOT EXISTS excluido BOOLEAN NOT NULL DEFAULT false;

