namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class TermosDeUsoConsultas
    {
        internal static string ObterUltimaVersaoDosTermosDeUso = @"
        SELECT 
            Id,
            descricao_termos_uso, 
            descricao_politica_privacidade, 
            versao, 
            criado_em, 
            criado_por, 
            alterado_em, 
            alterado_por
        FROM termos_de_uso tdu 
        ORDER BY versao DESC limit 1 ";
    }
}
