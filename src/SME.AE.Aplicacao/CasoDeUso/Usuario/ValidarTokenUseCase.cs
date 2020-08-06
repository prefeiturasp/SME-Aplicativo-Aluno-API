using MediatR;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenAutenticacao;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ValidarTokenUseCase : IValidarTokenUseCase
    {
        private readonly IMediator mediator;

        public ValidarTokenUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(ValidarTokenDto validarTokenDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorTokenAutenticacaoCommand(validarTokenDto.Token));

            if (usuario.ValidadeToken < DateTime.Now)
                throw new NegocioException("O Token não esta mais valido, solicite um novo token de autenticacao");

            return RespostaApi.Sucesso();
        }
    }
}
