-- Ensino Infantil
insert into parametroescolaaqui (chave, conteudo) 
select 'EnsinoInfantilFrequenciaEmAlertaFaixa', '60' where not exists (select 1 from parametroescolaaqui where chave = 'EnsinoInfantilFrequenciaEmAlertaFaixa');

insert into parametroescolaaqui (chave, conteudo)
select 'EnsinoInfantilFrequenciaRegularFaixa', '75' where not exists (select 1 from parametroescolaaqui where chave = 'EnsinoInfantilFrequenciaRegularFaixa');

insert into parametroescolaaqui (chave, conteudo)
select 'EnsinoInfantilFrequenciaInsuficienteCor', '#F6461F' where not exists (select 1 from parametroescolaaqui where chave = 'EnsinoInfantilFrequenciaInsuficienteCor');

insert into parametroescolaaqui (chave, conteudo)
select 'EnsinoInfantilFrequenciaEmAlertaCor', '#F5D00A' where not exists (select 1 from parametroescolaaqui where chave = 'EnsinoInfantilFrequenciaEmAlertaCor');

insert into parametroescolaaqui (chave, conteudo)
select 'EnsinoInfantilFrequenciaRegularCor', '#74C908' where not exists (select 1 from parametroescolaaqui where chave = 'EnsinoInfantilFrequenciaRegularCor');

-- Ensino Fundamental, Médio e EJA
insert into parametroescolaaqui (chave, conteudo)
select 'FrequenciaEmAlertaFaixa', '75' where not exists (select 1 from parametroescolaaqui where chave = 'FrequenciaEmAlertaFaixa');

insert into parametroescolaaqui (chave, conteudo)
select 'FrequenciaRegularFaixa', '80' where not exists (select 1 from parametroescolaaqui where chave = 'FrequenciaRegularFaixa');

insert into parametroescolaaqui (chave, conteudo)
select 'FrequenciaInsuficienteCor', '#F6461F' where not exists (select 1 from parametroescolaaqui where chave = 'FrequenciaInsuficienteCor');

insert into parametroescolaaqui (chave, conteudo)
select 'FrequenciaEmAlertaCor', '#F5D00A' where not exists (select 1 from parametroescolaaqui where chave = 'FrequenciaEmAlertaCor');

insert into parametroescolaaqui (chave, conteudo)
select 'FrequenciaRegularCor', '#74C908' where not exists (select 1 from parametroescolaaqui where chave = 'FrequenciaRegularCor');