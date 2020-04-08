namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class NotificacaoConsultas
    {
        public static string Select = @"
            SELECT Id, Mensagem, Titulo, Grupo, DataEnvio, DataExpiracao,
                   CriadoEm, CriadoPor, AlteradoEm, AlteradoPor
            FROM Notificacao 
        ";
    }
}