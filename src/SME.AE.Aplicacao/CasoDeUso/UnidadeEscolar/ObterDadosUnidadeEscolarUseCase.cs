using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using SME.AE.Aplicacao.Consultas.UnidadeEscolar;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class ObterDadosUnidadeEscolarUseCase : IObterDadosUnidadeEscolarUseCase
    {
        private readonly IMediator mediator;

        public ObterDadosUnidadeEscolarUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UnidadeEscolarResposta> Executar(string codigoUe)
        {
            return await mediator.Send(new ObterDadosUnidadeEscolarQuery(codigoUe));
        }
    }
}