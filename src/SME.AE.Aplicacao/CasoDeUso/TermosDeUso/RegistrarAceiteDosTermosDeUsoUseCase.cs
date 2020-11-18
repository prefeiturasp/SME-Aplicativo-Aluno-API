using MediatR;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using SME.AE.Comum.Excecoes;
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
            await mediator.Send(new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(aceite.CpfUsuario, aceite.Versao));

            await mediator.Send(new ValidarTermosDeUsoQuery(aceite.TermoDeUsoId));

            var usuario = await mediator.Send(new ObterUsuarioNaoExcluidoPorCpfQuery(aceite.CpfUsuario));
            if (usuario == null)
                throw new NegocioException("Não localizamos um usuário com o CPF informado");

            return await mediator.Send(new RegistrarAceiteDosTermosDeUsoCommand(aceite.TermoDeUsoId, aceite.CpfUsuario, aceite.Device, aceite.Ip, aceite.Versao));
        }
    }
}
