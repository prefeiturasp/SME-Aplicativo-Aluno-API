namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioConsultas
    {
        internal static string ObterPorCpf = @"
            SELECT Id, Cpf, Nome, UltimoLogin
            FROM Usuario
            WHERE Usuario.Cpf = @Cpf";
           //  AND Usuario.excluido = false";
    }
}
