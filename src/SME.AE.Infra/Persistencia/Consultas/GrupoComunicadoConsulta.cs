using System;

namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class GrupoComunicadoConsulta
    {
        public static String Select = @"
            SELECT id as Id, 
                nome as Nome,
                tipo_escola_id as TipoEscolaId,
                tipo_ciclo_id as TipoCicloId,
                etapa_ensino_id AS EtapaEnsinoId,
                criado_em as CriadoEm,
                criado_por as CriadoPor,
                alterado_em as AlteradoEm,
                alterado_por as AlteradoPor,
                criado_rf as CriadoRf,
                alterado_rf as AlteradoRf,
                excluido as Excluido
            FROM grupo_comunicado 
        ";
    }
}