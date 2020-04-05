using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class AutenticarUsuarioUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, Usuario usuario)
        {
            return await mediator.Send(new AutenticarUsuarioCommand(usuario.Cpf,usuario.DataNascimento));
        }
    }
}
