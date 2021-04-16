update parametroescolaaqui 
set conteudo = '*/10 * * * *'
where chave = 'RemoverConexaoIdle' and
	  conteudo <> '*/10 * * * *';