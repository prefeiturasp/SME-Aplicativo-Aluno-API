USE [CoreSSO]

BEGIN TRANSACTION

DECLARE @sis_id int = 1001; 
DECLARE @sis_entidade UNIQUEIDENTIFIER = '6CF424DC-8EC3-E011-9B36-00155D033206';
IF NOT EXISTS(SELECT 1 FROM SYS_SistemaEntidade WHERE sis_id = @sis_id)
BEGIN
INSERT INTO [dbo].[SYS_SistemaEntidade]
           ([sis_id]
           ,[ent_id]
           ,[sen_chaveK1]
           ,[sen_urlAcesso]
           ,[sen_logoCliente]
           ,[sen_urlCliente]
           ,[sen_situacao])
     VALUES
           (@sis_id
           ,@sis_entidade
           ,null
           ,null
           ,null
           ,null
           ,1)
END
