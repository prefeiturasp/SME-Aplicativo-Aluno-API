ALTER TABLE if exists public.nota_aluno ADD if not exists recomendacoes_aluno varchar;
ALTER TABLE if exists public.nota_aluno ADD if not exists recomendacoes_familia varchar;
ALTER TABLE if exists public.nota_aluno ADD if not exists nota_descricao varchar(50) NULL;