using SME.AE.Dominio.Entidades;
using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class BaseMap<T> : EntityMap<T> where T : EntidadeBase
    {
        public BaseMap()
        {
            Map(c => c.CriadoEm).ToColumn("criado_em");
            Map(c => c.CriadoPor).ToColumn("criado_por");
            Map(c => c.AlteradoEm).ToColumn("alterado_em");
            Map(c => c.AlteradoPor).ToColumn("alterado_por");
        }
    }
}
