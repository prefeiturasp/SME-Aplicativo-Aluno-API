using Dapper.FluentMap.Dommel.Mapping;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class BaseMap<T> : DommelEntityMap<T> where T : EntidadeBase
    {
        public BaseMap()
        {
            Map(c => c.CriadoEm).ToColumn("criadoem");
            Map(c => c.CriadoPor).ToColumn("criadopor");
            Map(c => c.AlteradoEm).ToColumn("alteradoem");
            Map(c => c.AlteradoPor).ToColumn("alteradopor");
        }
    }
}
