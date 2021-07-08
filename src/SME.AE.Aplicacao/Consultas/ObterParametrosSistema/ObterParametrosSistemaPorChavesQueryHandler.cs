using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterParametrosSistemaPorChavesQueryHandler : IRequestHandler<ObterParametrosSistemaPorChavesQuery, IEnumerable<ParametroEscolaAqui>>
    {
        private readonly IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio;

        public ObterParametrosSistemaPorChavesQueryHandler(IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio)
        {
            this.parametrosEscolaAquiRepositorio = parametrosEscolaAquiRepositorio ?? throw new System.ArgumentNullException(nameof(parametrosEscolaAquiRepositorio));
        }

        public async Task<IEnumerable<ParametroEscolaAqui>> Handle(ObterParametrosSistemaPorChavesQuery request, CancellationToken cancellationToken)
        {
            return await parametrosEscolaAquiRepositorio.ObterParametros(request.Chaves);
        }
    }
}
