using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida
{
    public class MarcarMensagemLidaUseCase
    {
        public static async Task<bool> Executar(IMediator mediator, UsuarioNotificacao usuarioMensagem)
        {
            return await mediator.Send(new UsuarioNotificacaoCommand(usuarioMensagem.Id,usuarioMensagem.UsuarioId));
        }
    }
}
