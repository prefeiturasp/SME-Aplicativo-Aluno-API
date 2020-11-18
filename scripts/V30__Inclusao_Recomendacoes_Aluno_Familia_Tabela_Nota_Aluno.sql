alter table public.nota_aluno
add column if not exists recomendacoes_aluno varchar(100),
add column if not exists recomendacoes_familia varchar(100)

