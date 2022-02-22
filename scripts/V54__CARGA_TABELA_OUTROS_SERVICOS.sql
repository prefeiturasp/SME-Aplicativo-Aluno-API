
-- Insert Icone Prato Feito
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Prato Aberto','Um Jeito fácil para todo mundo se nutir de informação sobre o que é servido na escola','Alimentação',
'https://pratoaberto.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/prato_aberto.png',
true,true,2,now(),'Sistema',null,null,1,null);

-- Insert Icone Material Escolar
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Material Escolar','Crédito para compra do material escolar no aplicativo','Material e Uniforme',
'https://portalmaterialescolar.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/material_escolar.png',
true,true,4,now(),'Sistema',null,null,1,null);

-- Insert Icone Uniformes
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Uniformes','Use o crédito para comprar o uniforme escolar em um fornecedor cadastrado.','Material e Uniforme',
'https://portaldeuniformes.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/uniformes.png',
true,true,5,now(),'Sistema',null,null,1,null);

-- Insert Icone Voltas às aula
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Voltas às aulas','Veja todos os detalhes sobre a volta às aulas.','Solicitações e Informações',
'https://educacao.sme.prefeitura.sp.gov.br/ano-letivo-2022/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/voltas_aulas.png',
true,true,1,now(),'Sistema',null,null,1,null);

-- Insert Icone  Escola aberta
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Escola aberta','É possível consultar os dados de escolas ou gerais, de toda a Rede Municipal de Educação.','Solicitações e Informações',
'https://escolaaberta.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/escola_aberta.png',
true,true,6,now(),'Sistema',null,null,1,null);


-- Insert Icone  NAAPA
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('NAAPA','Conheça o trabalho do Núcleo de Apoio e Acompanhamento para Aprendizegem(NAAPA).','Solicitações e Informações',
'https://turmadonaapa.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/naapa.png',
true,true,3,now(),'Sistema',null,null,1,null);


-- Insert Icone  SIC
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('SIC - Serviço de informações ao cidadão','O SIC recebe e registra pedidos de acesso à informação feitos por cidadãos.','Solicitações e Informações',
'http://esic.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/sic.png',
false,true,null,now(),'Sistema',null,null,1,null);

-- Insert Icone  Solicitação de vaga
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Solicitação de vaga','Preencha o formúlario online, para efetuar solicitações de vagas disponíveis.','Solicitações e Informações',
'https://vaganacreche.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/solicitacao_vaga.png',
false,true,null,now(),'Sistema',null,null,1,null);