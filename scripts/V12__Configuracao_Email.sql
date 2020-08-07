ALTER TABLE public.configuracao_email ADD IF NOT EXISTS criado_em timestamptz NOT NULL;
ALTER TABLE public.configuracao_email ADD IF NOT EXISTS criado_por varchar(200) NOT NULL;
ALTER TABLE public.configuracao_email ADD IF NOT EXISTS alterado_em timestamptz NULL;
ALTER TABLE public.configuracao_email ADD IF NOT EXISTS alterado_por varchar(200) NULL;

insert
	into
	public.configuracao_email ( 
	email_remetente,
	nome_remetente,
	servidor_smtp,
	usuario,
	senha,
	porta,
	tls,
	criado_em,
	criado_por)
select  
'sgp-nao_responder@sme.prefeitura.sp.gov.br',
'Escola Aqui - Secretaria Municipal de Educação de São Paulo',
'smtp.office365.com',
'sgp-nao_responder@sme.prefeitura.sp.gov.br',
'Definir manualmente',
587,
false,
NOW(),
'Sistema'
where
	not exists(
	select
		1
	from
		public.configuracao_email);