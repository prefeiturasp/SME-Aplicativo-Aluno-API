using Dapper.FluentMap.Dommel.Mapping;
using SME.AE.Dominio.Entidades.Externas;

namespace SME.AE.Infra.Persistencia.Mapeamentos.Externos
{
    public class ExternoMap<T> : DommelEntityMap<T> where T : EntidadeExterna
    {
    }
}
