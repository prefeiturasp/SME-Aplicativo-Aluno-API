USE CoreSSO
GO

DECLARE @sis_id INT = 1001

----------------------------------------------MODULO----------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SYS_Modulo WHERE sis_id = @sis_id AND mod_id = 1)
	INSERT INTO [dbo].[SYS_Modulo]([sis_id],[mod_id],[mod_nome],[mod_auditoria],[mod_situacao])VALUES(@sis_id,1,'Geral',0,1);
----------------------------------------------MODULO----------------------------------------------

----------------------------------------------MODULO SITE MAP----------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SYS_ModuloSiteMap WHERE sis_id = @sis_id AND mod_id = 1)
INSERT INTO [dbo].[SYS_ModuloSiteMap]([sis_id],[mod_id],[msm_id],[msm_nome],[msm_descricao],[msm_url])VALUES(@sis_id,1,1,'Geral',null,'~/');
----------------------------------------------MODULO SITE MAP----------------------------------------------

----------------------------------------------VISAO----------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SYS_Visao WHERE vis_id =11)
	INSERT INTO [dbo].[SYS_Visao]([vis_id],[vis_nome])VALUES (11,'Escola Aqui');
----------------------------------------------VISAO----------------------------------------------

----------------------------------------------VISAO MODULO----------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SYS_VisaoModulo WHERE sis_id = @sis_id AND vis_id IN (1,2,3,4,11))
	INSERT INTO SYS_VisaoModulo (vis_id,sis_id,mod_id)
	SELECT vis.vis_id,modulo.sis_id,modulo.mod_id
	FROM SYS_Modulo AS modulo
	CROSS APPLY(
		SELECT visao.vis_id FROM SYS_Visao AS visao) AS vis
		WHERE modulo.sis_id = 1001 and vis.vis_id IN (1,2,3,4,11)
----------------------------------------------VISAO MODULO----------------------------------------------		

----------------------------------------------VISAO MODULO MENU----------------------------------------------
IF NOT EXISTS (SELECT 1 FROM SYS_VisaoModuloMenu WHERE sis_id = @sis_id AND vis_id IN (1,2,3,4,11))
BEGIN
	INSERT INTO SYS_VisaoModuloMenu(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
	SELECT 1 AS vis_id, visaomodulo.sis_id,visaomodulo.mod_id,visaomodulo.msm_id AS msm_id,1 AS vmm_ordem
	FROM SYS_ModuloSiteMap AS visaomodulo WHERE visaomodulo.sis_id = @sis_id

	INSERT INTO SYS_VisaoModuloMenu(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
	SELECT 2 AS vis_id, visaomodulo.sis_id,visaomodulo.mod_id,visaomodulo.msm_id AS msm_id,1 AS vmm_ordem
	FROM SYS_ModuloSiteMap AS visaomodulo WHERE visaomodulo.sis_id = @sis_id

	INSERT INTO SYS_VisaoModuloMenu(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
	SELECT 3 AS vis_id, visaomodulo.sis_id,visaomodulo.mod_id,visaomodulo.msm_id AS msm_id,1 AS vmm_ordem
	FROM SYS_ModuloSiteMap AS visaomodulo WHERE visaomodulo.sis_id = @sis_id

	INSERT INTO SYS_VisaoModuloMenu(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
	SELECT 4 AS vis_id, visaomodulo.sis_id,visaomodulo.mod_id,visaomodulo.msm_id AS msm_id,1 AS vmm_ordem
	FROM SYS_ModuloSiteMap AS visaomodulo WHERE visaomodulo.sis_id = @sis_id

	INSERT INTO SYS_VisaoModuloMenu(vis_id,sis_id,mod_id,msm_id,vmm_ordem)
	SELECT 11 AS vis_id, visaomodulo.sis_id,visaomodulo.mod_id,visaomodulo.msm_id AS msm_id,1 AS vmm_ordem
	FROM SYS_ModuloSiteMap AS visaomodulo WHERE visaomodulo.sis_id = @sis_id
END
----------------------------------------------VISAO MODULO MENU----------------------------------------------
