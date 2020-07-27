using SME.AE.Dominio.Entidades;
using Dapper.FluentMap.Dommel.Mapping;

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
