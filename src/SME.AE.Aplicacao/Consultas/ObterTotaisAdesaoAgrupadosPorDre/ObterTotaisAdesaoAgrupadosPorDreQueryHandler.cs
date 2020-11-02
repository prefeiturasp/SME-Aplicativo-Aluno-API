using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterTotaisAdesaoAgrupadosPorDre
{
    public class ObterTotaisAdesaoAgrupadosPorDreQueryHandler : IRequestHandler<ObterTotaisAdesaoAgrupadosPorDreQuery, IEnumerable<TotaisAdesaoResultado>>
    {
        private readonly IAdesaoRepositorio adesaoRepositorio;

        public ObterTotaisAdesaoAgrupadosPorDreQueryHandler(IAdesaoRepositorio adesaoRepositorio)
        {
            this.adesaoRepositorio = adesaoRepositorio ?? throw new System.ArgumentNullException(nameof(adesaoRepositorio));
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> Handle(ObterTotaisAdesaoAgrupadosPorDreQuery request, CancellationToken cancellationToken)
        {
            return await adesaoRepositorio.ObterDadosAdesaoAgrupadosPorDre();
        }
    }
}
