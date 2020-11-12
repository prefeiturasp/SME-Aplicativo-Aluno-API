using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    public abstract class RepositorioAppComCacheBase : RepositorioAppBase
    {
        protected readonly ICacheRepositorio _cacheRepositorio;

        public RepositorioAppComCacheBase(ICacheRepositorio cacheRepositorio)
        {
            _cacheRepositorio = cacheRepositorio;
        }
    }
}