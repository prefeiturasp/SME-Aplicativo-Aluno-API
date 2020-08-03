namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioConsultas
    {
        internal static string ObterPorCpf = @"
            SELECT Cpf,Celular,Nome,Email,UltimoLogin,PrimeiroAcesso,
                Excluido,Id,CriadoEm,CriadoPor,AlteradoEm,AlteradoPor
            FROM Usuario
            WHERE Usuario.Cpf = @Cpf";

        internal static string ObterTodos = @"
            SELECT Cpf from Usuario";

           //  AND Usuario.excluido = false";
    }
}
