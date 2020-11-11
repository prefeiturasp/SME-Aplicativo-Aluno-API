create table if not exists nota_aluno_cor (
nota VARCHAR(20) not null,
cor VARCHAR(7) not null);

CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS nota_aluno_cor_nota_idx 
ON public.nota_aluno_cor (nota);

    
insert into nota_aluno_cor (nota, cor) values ('NS', '#F6461F');
insert into nota_aluno_cor (nota, cor) values ('PS', '#74C908');
insert into nota_aluno_cor (nota, cor) values ('NotaAbaixo5', '#F6461F');
insert into nota_aluno_cor (nota, cor) values ('NotaEntre7e5', '#F5D00A');
insert into nota_aluno_cor (nota, cor) values ('NotaAcimaDe75', '#74C908');

alter table nota_aluno add nota_descricao varchar(50) null;
