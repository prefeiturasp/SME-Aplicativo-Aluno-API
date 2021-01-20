using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterTotaisAdesao
{
    public class ObterTotaisAdesaoQueryHandler : IRequestHandler<ObterTotaisAdesaoQuery, IEnumerable<TotaisAdesaoResultado>>
    {
        private readonly IAdesaoRepositorio adesaoRepositorio;

        public ObterTotaisAdesaoQueryHandler(IAdesaoRepositorio adesaoRepositorio)
        {
            this.adesaoRepositorio = adesaoRepositorio ?? throw new System.ArgumentNullException(nameof(adesaoRepositorio));
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> Handle(ObterTotaisAdesaoQuery request, CancellationToken cancellationToken)
        {
            return await adesaoRepositorio.ObterDadosAdesaoSintetico(request.CodigoDre, request.CodigoUe);
        }
    }
}
