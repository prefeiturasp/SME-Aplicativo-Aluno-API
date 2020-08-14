using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao;
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
            var usuario = await mediator.Send(new ObterUsuarioPorTokenRedefinicaoQuery(validarTokenDto.Token.ToUpper()));

            usuario.ValidarTokenRedefinicao(validarTokenDto.Token.ToUpper());

            return RespostaApi.Sucesso();
        }
    }
}
