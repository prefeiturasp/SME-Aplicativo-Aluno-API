using MediatR;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AlterarEmailCelularUseCase : IAlterarEmailCelularUseCase
    {
        public async Task<RespostaApi> Executar(IMediator mediator, AlterarEmailCelularDto alterarEmailCelularDto)
        {
            await mediator.Send(new AlterarEmailCelularCommand(alterarEmailCelularDto));

            var usuario = await mediator.Send(new ObterUsuarioQuery() { Id = alterarEmailCelularDto.Id });

            var token = await mediator.Send(new CriarTokenCommand(usuario.Cpf));

            return RespostaApi.Sucesso(new RetornoToken(token));
        }
    }
}
