using MediatR;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.TermosDeUso
{
    public class RegistrarAceiteDosTermosDeUsoUseCase : IRegistrarAceiteDosTermosDeUsoUseCase
    {
        private readonly IMediator mediator;

        public RegistrarAceiteDosTermosDeUsoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(RegistrarAceiteDosTermosDeUsoDto aceite)
        {
            await mediator.Send(new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(aceite.Usuario, aceite.Versao));

            await mediator.Send(new ValidarTermosDeUsoQuery(aceite.TermoDeUsoId));

            return await mediator.Send(new RegistrarAceiteDosTermosDeUsoCommand(aceite.TermoDeUsoId, aceite.Usuario, aceite.Device, aceite.Ip, aceite.Versao));
        }
    }
}
