using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    public abstract class RepositorioComCacheBase : RepositorioBase
    {
        protected readonly ICacheRepositorio _cacheRepositorio;

        public RepositorioComCacheBase(ICacheRepositorio cacheRepositorio)
        {
            _cacheRepositorio = cacheRepositorio;
        }
    }
}