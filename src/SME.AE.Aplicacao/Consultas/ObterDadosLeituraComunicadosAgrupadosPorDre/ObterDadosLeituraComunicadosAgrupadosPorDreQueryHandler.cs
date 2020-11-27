using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosAgrupadosPorDreQueryHandler : IRequestHandler<ObterDadosLeituraComunicadosAgrupadosPorDreQuery, IEnumerable<DadosLeituraComunicadosResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;

        public ObterDadosLeituraComunicadosAgrupadosPorDreQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Handle(ObterDadosLeituraComunicadosAgrupadosPorDreQuery request, CancellationToken cancellationToken)
        {
            var dadosConsolidados = await dadosLeituraRepositorio.ObterDadosLeituraComunicadosPorDre(request.NotificaoId);

            return null;
        }
    }
}
