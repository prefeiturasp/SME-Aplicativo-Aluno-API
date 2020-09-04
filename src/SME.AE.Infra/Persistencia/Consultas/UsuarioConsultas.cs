namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class UsuarioConsultas
    {
        internal static string ObterPorCpf = $@"
            SELECT pf,Celular,Nome,Email,UltimoLogin,PrimeiroAcesso,
                Excluido,Id,CriadoEm,CriadoPor,AlteradoEm,AlteradoPor,
                token_redefinicao, redefinicao, validade_token
            FROM Usuario
            WHERE Usuario.Cpf = @Cpf";

        internal static string ObterTodos = @"
            SELECT Cpf from Usuario";

        internal static string ObterCampos = @"Cpf,Celular,Nome,Email,UltimoLogin,PrimeiroAcesso,
                Excluido,Id,CriadoEm,CriadoPor,AlteradoEm,AlteradoPor,
                token_redefinicao, redefinicao, validade_token";

           //  AND Usuario.excluido = false";
    }
}
