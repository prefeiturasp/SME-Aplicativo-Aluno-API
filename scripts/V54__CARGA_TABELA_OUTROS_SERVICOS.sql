
-- Insert Icone Prato Feito
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Prato Aberto','Um Jeito f�cil para todo mundo se nutir de informa��o sobre o que � servido na escola','Alimenta��o',
'https://pratoaberto.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/prato_aberto.png',
true,true,2,now(),'Sistema',null,null,1,null);

-- Insert Icone Material Escolar
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Material Escolar','Cr�dito para compra do material escolar no aplicativo','Material e Uniforme',
'https://portalmaterialescolar.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/material_escolar.png',
true,true,4,now(),'Sistema',null,null,1,null);

-- Insert Icone Uniformes
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Uniformes','Use o cr�dito para comprar o uniforme escolar em um fornecedor cadastrado.','Material e Uniforme',
'https://portaldeuniformes.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/uniformes.png',
true,true,5,now(),'Sistema',null,null,1,null);

-- Insert Icone Voltas �s aula
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Voltas �s aulas','Veja todos os detalhes sobre a volta �s aulas.','Solicita��es e Informa��es',
'https://educacao.sme.prefeitura.sp.gov.br/ano-letivo-2022/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/voltas_aulas.png',
true,true,1,now(),'Sistema',null,null,1,null);

-- Insert Icone  Escola aberta
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Escola aberta','� poss�vel consultar os dados de escolas ou gerais, de toda a Rede Municipal de Educa��o.','Solicita��es e Informa��es',
'https://escolaaberta.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/escola_aberta.png',
true,true,6,now(),'Sistema',null,null,1,null);


-- Insert Icone  NAAPA
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('NAAPA','Conhe�a o trabalho do N�cleo de Apoio e Acompanhamento para Aprendizegem(NAAPA).','Solicita��es e Informa��es',
'https://turmadonaapa.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/naapa.png',
true,true,3,now(),'Sistema',null,null,1,null);


-- Insert Icone  SIC
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('SIC - Servi�o de informa��es ao cidad�o','O SIC recebe e registra pedidos de acesso � informa��o feitos por cidad�os.','Solicita��es e Informa��es',
'http://esic.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/sic.png',
false,true,null,now(),'Sistema',null,null,1,null);

-- Insert Icone  Solicita��o de vaga
insert into public.outroservico(titulo,descricao,categoria,urlsite,icone,destaque,ativo,ordem,criado_em,criado_por,alterado_em,alterado_por,criado_rf,alterado_rf)
values('Solicita��o de vaga','Preencha o form�lario online, para efetuar solicita��es de vagas dispon�veis.','Solicita��es e Informa��es',
'https://vaganacreche.sme.prefeitura.sp.gov.br/','https://novosgp.sme.prefeitura.sp.gov.br/Arquivos/EscolaAqui/solicitacao_vaga.png',
false,true,null,now(),'Sistema',null,null,1,null);