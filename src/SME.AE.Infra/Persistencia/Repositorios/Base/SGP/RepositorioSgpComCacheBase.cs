using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Infra.Persistencia.Repositorios.Base.SGP
{
    // TO DO: Colocar os demais reposiórios para herdar da classe base
    public class RepositorioSgpComCacheBase : RepositorioSgpBase
    {
        protected readonly ICacheRepositorio _cacheRepositorio;

        public RepositorioSgpComCacheBase(ICacheRepositorio cacheRepositorio)
        {
            _cacheRepositorio = cacheRepositorio;
        }
    }
}