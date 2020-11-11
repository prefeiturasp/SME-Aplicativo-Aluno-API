create table if not exists frequencia_aluno_cor (
frequencia VARCHAR(20) not null,
cor VARCHAR(7) not null);

CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS nota_aluno_cor_nota_idx 
ON public.nota_aluno_cor (nota);

delete from frequencia_aluno_cor;
insert into frequencia_aluno_cor (frequencia, cor) values ('NS', '#F6461F');
insert into frequencia_aluno_cor (frequencia, cor) values ('PS', '#74C908');
insert into frequencia_aluno_cor (frequencia, cor) values ('NotaAbaixo5', '#F6461F');
insert into frequencia_aluno_cor (frequencia, cor) values ('NotaEntre7e5', '#F5D00A');
insert into frequencia_aluno_cor (frequencia, cor) values ('NotaAcimaDe75', '#74C908');