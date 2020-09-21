namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class TermosDeUsoConsultas
    {
        internal static string ObterTermosDeUso = @"
        SELECT 
            Id,
            descricao_termos_uso, 
            descricao_politica_privacidade, 
            versao, 
            criado_em, 
            criado_por, 
            alterado_em, 
            alterado_por
        FROM termos_de_uso tdu ";

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

        internal static string ObterTermosDeUsoPorCpf = @"
        SELECT 
            tdu.Id,
            tdu.descricao_termos_uso, 
            tdu.descricao_politica_privacidade, 
            tdu.versao, 
            tdu.criado_em, 
            tdu.criado_por, 
            tdu.alterado_em, 
            tdu.alterado_por
        FROM termos_de_uso tdu 
        WHERE tdu.Id NOT IN (select termos_de_uso_id from aceite_termos_de_uso atdu where cpf_usuario = @cpf)
        ORDER BY tdu.versao DESC limit 1 ";

    }
}
