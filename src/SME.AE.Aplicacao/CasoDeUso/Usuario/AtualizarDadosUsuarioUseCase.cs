using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioUseCase : IAtualizarDadosUsuarioUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(AtualizarDadosUsuarioDto usuarioDto)
        {
            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(usuarioDto.Id));

            if (usuarioApp == null)
                return RespostaApi.Falha("Usuário não encontrado!");

            var usuarioEol = await mediator.Send(new ObterDadosResumidosReponsavelPorCpfQuery(usuarioApp.Cpf));

            if (usuarioEol == null)
                return RespostaApi.Falha("Usuário não encontrado!");

            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralEol, usuarioDto, Guid.NewGuid()));
            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralProdam, usuarioDto, Guid.NewGuid()));

            return RespostaApi.Sucesso();
        }
    }
}
