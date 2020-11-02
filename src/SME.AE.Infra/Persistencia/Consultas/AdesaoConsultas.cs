namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AdesaoConsultas
    {
        internal static string ObterDadosAdesaoAgrupadosPorDreUe = @"
		SELECT 	
			concat(dre_codigo,'-',dre_nome) AS NomeCompletoDre,
			null AS NomeCompletoUe,
			null AS CodigoTurma, 
			sum(usuarios_cpf_invalidos) AS TotalUsuariosComCpfInvalidos,
			sum(usuarios_sem_app_instalado) AS TotalUsuariosSemAppInstalado, 
			sum(usuarios_primeiro_acesso_incompleto) AS TotalUsuariosPrimeiroAcessoIncompleto, 
			sum(usuarios_validos) AS TotalUsuariosValidos 
		FROM dashboard_adesao da  
		WHERE 1=1 ";

		internal static string ObterDadosAdesaoSme = @"
		SELECT 	
			null AS NomeCompletoDre,
			null AS NomeCompletoUe,
			null AS CodigoTurma, 
			sum(usuarios_cpf_invalidos) AS TotalUsuariosComCpfInvalidos,
			sum(usuarios_sem_app_instalado) AS TotalUsuariosSemAppInstalado, 
			sum(usuarios_primeiro_acesso_incompleto) AS TotalUsuariosPrimeiroAcessoIncompleto, 
			sum(usuarios_validos) AS TotalUsuariosValidos 
		FROM dashboard_adesao da  
		WHERE 1=1 ";

	}
}
