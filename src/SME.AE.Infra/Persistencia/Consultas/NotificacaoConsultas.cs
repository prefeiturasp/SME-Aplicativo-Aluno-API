namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class NotificacaoConsultas
    {
        public static string Select = @"
            SELECT Id, Mensagem, Titulo, Grupo, DataEnvio, DataExpiracao,
                   CriadoEm, CriadoPor, AlteradoEm, AlteradoPor, Dre_CodigoEol, Ue_CodigoEol, TipoComunicado, CategoriaNotificacao
            FROM Notificacao 
        ";

        public static string ObterPorUsuarioLogado = @"select 
                                                            distinct 
                                                            N.Id,
	                                                        N.Mensagem,
	                                                        N.Titulo,
	                                                        N.Grupo,
	                                                        N.DataEnvio,
	                                                        N.DataExpiracao,
	                                                        N.CriadoEm,
	                                                        N.CriadoPor,
	                                                        N.AlteradoEm,
	                                                        N.AlteradoPor,
                                                            N.TipoComunicado,
                                                            N.CategoriaNotificacao,
                                                            N.SeriesResumidas,
	                                                        UNL.mensagemvisualizada
                                                        from
	                                                        Notificacao N
                                                        left join usuario_notificacao_leitura UNL on
	                                                        UNL.notificacao_id = N.id ";
    }
}
