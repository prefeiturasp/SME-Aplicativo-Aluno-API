using SME.AE.Dominio.Comum.Enumeradores;

namespace SME.AE.Infra.Persistencia.Comandos
{
    public static class CoreSSOComandos
    {
        internal static string InserirUsuario = @"
            INSERT INTO SYS_Usuario
                (usu_id
                , usu_login
                , usu_senha
                , usu_criptografia
                , usu_situacao
                , usu_dataCriacao
                , usu_dataAlteracao
                , pes_id
                , usu_integridade
                , ent_id
                , usu_integracaoAD
                , usu_dataAlteracaoSenha)
            VALUES
                (@usuId
                , @login
                , @senha
                ," + (byte)TipoCriptografia.TripleDES + @"
                ,1
                , GETDATE()
                , GETDATE()
                , @pessoaId
                ,0
                ,'6CF424DC-8EC3-E011-9B36-00155D033206'
                ,0
                , GETDATE())";
        internal static string InserirPessoa = @"
            INSERT INTO PES_Pessoa
                (pes_id
                , pes_nome
                , pes_situacao
                , pes_dataCriacao
                , pes_dataAlteracao
                , pes_integridade)
            VALUES
                (@pessoaId
                , @pesNome
                ,1
                , GETDATE()
                , GETDATE()
                ,0)";
        internal static string InserirPessoaDocumento = @"
            INSERT INTO PES_PessoaDocumento
                (pes_id
                ,tdo_id
                ,psd_numero
                ,psd_situacao
                ,psd_dataCriacao
                ,psd_dataAlteracao)
            VALUES
                (@pessoaId
                ,'2CEEED03-63EB-E011-9B36-00155D033206'
                ,@cpf
                ,1
                ,GETDATE()
                ,GETDATE())";

        public static string InserirUsuarioGrupo = @"
            INSERT INTO SYS_UsuarioGrupo
                (usu_id
                ,gru_id
                ,usg_situacao)
            VALUES
                (@usuId
                ,@gruId
                ,1)";
        public static string AtualizarStatusUsuario = @"
            UPDATE SYS_Usuario SET usu_situacao = @status WHERE usu_id = @usuId";
        public static string AtualizarStatusUsuarioGrupo = @"
            UPDATE UG 
            SET ug.usg_situacao = @status 
                FROM SYS_UsuarioGrupo UG
                    INNER JOIN SYS_Grupo G
                        ON UG.gru_id = g.gru_id
            WHERE usu_id = @usuId
                AND g.sis_id = 1001";

        public static string AtualizarCriptografia = @"
            UPDATE SYS_Usuario SET usu_criptografia = 1, usu_senha = @senha
            WHERE usu_id = @usuId
        ";

    }
}
