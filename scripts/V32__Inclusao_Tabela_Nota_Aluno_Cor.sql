create table if not exists nota_aluno_cor (
nota VARCHAR(20) not null,
cor VARCHAR(7) not null);

alter table nota_aluno add if not exists nota_descricao varchar(50) null;
