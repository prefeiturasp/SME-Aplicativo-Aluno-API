using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos
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
            if (string.IsNullOrEmpty(request.CodigoDre) && string.IsNullOrEmpty(request.CodigoUe))
                return await adesaoRepositorio.ObterDadosAdesaoSme();

            return await adesaoRepositorio.ObterDadosAdesaoAgrupadosPorDreUeETurma(request.CodigoDre, request.CodigoUe);
        }
    }
}
