using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterDadosDeLeituraComunicadosUseCase : IObterDadosDeLeituraComunicadosUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraComunicadosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<DadosLeituraResultado>> Executar(string codigoDre, string codigoUe)
        {
            throw new NotImplementedException();
        }
    }
}
