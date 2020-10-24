namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AdesaoConsultas
    {
        internal static string ObterDadosAdesao = @"
		SELECT
			 dre_codigo as CodigoDre
			,ue_codigo as CodigoUe
			,codigo_turma as CodigoTurma
			,usuarios_cpf_invalidos as TotalUsuariosComCpfInvalidos
			,usuarios_primeiro_acesso_incompleto as TotalUsuariosPrimeiroAcesso
			,usuarios_sem_app_instalado as TotalUsuariosSemAppInstalado
			,usuarios_validos as TotalUsuariosValidos
		FROM dashboard_adesao 
		WHERE  1=1 ";
    }
}
