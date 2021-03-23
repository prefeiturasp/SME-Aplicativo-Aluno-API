insert into parametroescolaaqui (chave, conteudo)
select 'RemoverConexaoIdle' chave, '*/30 * * * *' conteudo
where not exists (select 1
				  	from parametroescolaaqui
				  where chave = 'RemoverConexaoIdle');