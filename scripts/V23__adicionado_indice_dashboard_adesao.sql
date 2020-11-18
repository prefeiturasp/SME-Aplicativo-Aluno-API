alter table if exists dashboard_adesao
alter column dre_codigo set not null,
alter column dre_codigo drop default,
alter column ue_codigo set not null,
alter column ue_codigo drop default,
alter column codigo_turma set not null,
alter column codigo_turma drop default;

