USE CoreSSO
GO

DECLARE @sis_id int = 1001; 
DECLARE @sis_caminho varchar(30) = 'https://appaluno.sme.prefeitura.sp.gov.br/'; 
DECLARE @sis_caminhoLogout varchar(30) = 'https://appaluno.sme.prefeitura.sp.gov.br/'; 
DECLARE @sis_nome varchar(30) = 'Escola Aqui';
DECLARE @sis_descricao varchar(30) = 'Escola Aqui';

IF NOT EXISTS(SELECT 1 FROM SYS_Sistema WHERE sis_id = @sis_id)
BEGIN
INSERT INTO [dbo].[SYS_Sistema]
           ([sis_id]
           ,[sis_nome]
           ,[sis_descricao]
           ,[sis_caminho]
           ,[sis_urlImagem]
           ,[sis_urlLogoCabecalho]
           ,[sis_tipoAutenticacao]
           ,[sis_urlIntegracao]
           ,[sis_situacao]
           ,[sis_caminhoLogout]
           ,[sis_ocultarLogo])
     VALUES
           (@sis_id
           ,@sis_nome
           ,@sis_descricao
           ,@sis_caminho
           ,null
           ,null
           ,1
           ,null
           ,1
           ,@sis_caminhoLogout
           ,0)
END
