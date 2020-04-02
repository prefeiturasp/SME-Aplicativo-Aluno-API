using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class CriarUsuarioUseCase
    {
        public static async Task<RespostaAutenticar> Executar(IMediator mediator)
        {
            return await mediator.Send(new CriarUsuarioCommand());
        }
    }
}
