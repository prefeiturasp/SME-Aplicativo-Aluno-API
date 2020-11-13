namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AdesaoConsultas
    {
        internal static string ObterDadosAdesao = @"
		SELECT 	
			trim(concat(dre_codigo,'-',dre_nome)) AS NomeCompletoDre,
			trim(concat(ue_codigo,'-',ue_nome)) AS NomeCompletoUe,
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

		internal static string ObterDadosAdesaoPorDre = @"
			SELECT 
				dre_nome AS NomeCompletoDre,
				usuarios_cpf_invalidos AS TotalUsuariosComCpfInvalidos,
				usuarios_sem_app_instalado AS TotalUsuariosSemAppInstalado, 
				usuarios_primeiro_acesso_incompleto AS TotalUsuariosPrimeiroAcessoIncompleto, 
				usuarios_validos AS TotalUsuariosValidos 
			FROM dashboard_adesao da  
			WHERE 
				codigo_turma = 0 and
				ue_codigo  = '' and
				dre_codigo <> '' 
			ORDER BY dre_codigo 
		";
	}
}
