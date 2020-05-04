namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioConsultas
    {
        internal static string ObterPorCpf = @"
            SELECT Id, Cpf, Nome, UltimoLogin
            FROM Usuario
            WHERE Usuario.Cpf = @Cpf";

        internal static string ObterTodos = @"
            SELECT Cpf from Usuario";

           //  AND Usuario.excluido = false";
    }
}
