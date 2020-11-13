using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    // TO DO: Colocar os demais reposiórios para herdar da classe base
    public abstract class RepositorioAppComCacheBase : RepositorioAppBase
    {
        protected readonly ICacheRepositorio _cacheRepositorio;

        public RepositorioAppComCacheBase(ICacheRepositorio cacheRepositorio)
        {
            _cacheRepositorio = cacheRepositorio;
        }
    }
}