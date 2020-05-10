USE CoreSSO
GO

DECLARE @sis_id int = 1001
IF NOT EXISTS(SELECT 1 FROM SYS_Grupo WHERE sis_id = @sis_id and gru_id = '95AA09D8-E1F8-45F9-A8CA-8DA79EBA5F98')
	INSERT INTO [dbo].[SYS_Grupo]([gru_id],[gru_nome],[gru_situacao],[vis_id],[sis_id],[gru_integridade])VALUES('95AA09D8-E1F8-45F9-A8CA-8DA79EBA5F98','Geral',1,11,@sis_id,0);


INSERT INTO SYS_GrupoPermissao(gru_id,sis_id,mod_id,grp_consultar,grp_inserir,grp_alterar,grp_excluir) SELECT gru.gru_id,gru.sis_id, 1 as mod_id , 1 as grp_consulta, 1 as grp_inserir, 1 as grp_alterar, 1 as grp_excluir  FROM SYS_Grupo as gru where gru.sis_id = @sis_id and gru_id = '95AA09D8-E1F8-45F9-A8CA-8DA79EBA5F98'