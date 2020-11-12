create table if not exists frequencia_aluno_cor (
frequencia VARCHAR(30) not null,
cor VARCHAR(7) not null);

CREATE UNIQUE INDEX IF NOT EXISTS frequencia_aluno_cor_nota_idx 
ON public.frequencia_aluno_cor (frequencia);