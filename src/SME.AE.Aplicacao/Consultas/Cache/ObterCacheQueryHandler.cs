using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterCacheQueryHandler : IRequestHandler<ObterCacheQuery, string>
    {
        private readonly ICacheRepositorio cacheRepositorio;

        public ObterCacheQueryHandler(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio ?? throw new System.ArgumentNullException(nameof(cacheRepositorio));
        }

       public async Task<string> Handle(ObterCacheQuery request, CancellationToken cancellationToken) { 
            await  cacheRepositorio.SalvarAsync("$teste", request.Cpf, 1, false);
            return await cacheRepositorio.ObterAsync("$teste");
        }
    }
}
