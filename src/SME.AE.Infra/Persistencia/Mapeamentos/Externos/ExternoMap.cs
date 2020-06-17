using Dapper.FluentMap.Mapping;
using SME.AE.Dominio.Entidades.Externas;

namespace SME.AE.Infra.Persistencia.Mapeamentos.Externos
{
    public class ExternoMap<T> : EntityMap<T> where T : EntidadeExterna
    {
    }
}
