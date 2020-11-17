drop index if exists frequencia_aluno_ue_turma_ano_bimestre_aluno_idx;

alter table if exists frequencia_aluno
alter column ue_codigo type varchar(15);

alter table if exists frequencia_aluno
alter column turma_codigo type varchar(15);

alter table if exists frequencia_aluno
add column if not exists componente_curricular_codigo int8 NOT null default 0,
add column if not exists dias_ausencias varchar(1000) NOT null default '';

create index if not exists frequencia_aluno_ue_turma_ano_bimestre_disciplina_aluno_idx on frequencia_aluno (ue_codigo, turma_codigo, ano_letivo, bimestre, componente_curricular_codigo, aluno_codigo);

