using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Aluno
{
    public class CacheUseCase : ICacheUseCase
    {
        private readonly IMediator mediator;

        public CacheUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(string cpf)
        {
            var result = await mediator.Send(new ObterCacheQuery(cpf));
            return result;
        }
    }
}
