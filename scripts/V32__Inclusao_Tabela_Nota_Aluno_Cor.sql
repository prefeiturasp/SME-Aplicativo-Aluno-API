create table if not exists nota_aluno_cor (
nota VARCHAR(20) not null,
cor VARCHAR(7) not null);

CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS nota_aluno_cor_nota_idx 
ON public.nota_aluno_cor (nota);

    
insert into nota_aluno_cor (nota, cor) values ('NS', '#FF0000');
insert into nota_aluno_cor (nota, cor) values ('PS', '#008000');
insert into nota_aluno_cor (nota, cor) values ('NotaAbaixo5', '#FF0000');
insert into nota_aluno_cor (nota, cor) values ('NotaEntre7e5', '#FFFF00');
insert into nota_aluno_cor (nota, cor) values ('NotaAcimaDe75', '#008000');

alter table nota_aluno add nota_descricao varchar(50) null;
