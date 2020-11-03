namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AdesaoConsultas
    {
        internal static string ObterDadosAdesaoAgrupadosPorDreUe = @"
		SELECT 	
			concat(dre_codigo,'-',dre_nome) AS NomeCompletoDre,
			concat(ue_codigo,'-',ue_nome) AS NomeCompletoUe,
			codigo_turma AS CodigoTurma, 
				usuarios_cpf_invalidos AS TotalUsuariosComCpfInvalidos,
				usuarios_sem_app_instalado AS TotalUsuariosSemAppInstalado, 
				usuarios_primeiro_acesso_incompleto AS TotalUsuariosPrimeiroAcessoIncompleto, 
				usuarios_validos AS TotalUsuariosValidos 
		FROM dashboard_adesao da  
			WHERE 
				codigo_turma = 0 and
				dre_codigo = @dre_codigo and
				ue_codigo  = @ue_codigo
		";

		internal static string ObterDadosAdesaoSme = @"
			SELECT 
				usuarios_cpf_invalidos AS TotalUsuariosComCpfInvalidos,
				usuarios_sem_app_instalado AS TotalUsuariosSemAppInstalado, 
				usuarios_primeiro_acesso_incompleto AS TotalUsuariosPrimeiroAcessoIncompleto, 
				usuarios_validos AS TotalUsuariosValidos 
			FROM dashboard_adesao da  
			WHERE 
				codigo_turma = 0 and
				dre_codigo = '' and
				ue_codigo  = ''
		";

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

		internal static string ObterDadosAdesaoSintetico = @"
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
