alter table if exists frequencia_aluno
add column if not exists ano_letivo int2 NOT null default 0;