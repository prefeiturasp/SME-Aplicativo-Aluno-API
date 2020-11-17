using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosQueryHandler : IRequestHandler<ObterDadosLeituraComunicadosQuery, IEnumerable<DadosLeituraComunicadosResultado>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;

        public ObterDadosLeituraComunicadosQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new System.ArgumentNullException(nameof(dadosLeituraRepositorio));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Handle(ObterDadosLeituraComunicadosQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
