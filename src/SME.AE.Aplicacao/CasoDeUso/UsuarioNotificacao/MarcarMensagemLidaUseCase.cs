using MediatR;
using Sentry.Protocol;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Notificacao;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida
{
    public class MarcarMensagemLidaUseCase
    {
        public static async Task<NotificacaoResposta> Executar(IMediator mediator, Comum.Modelos.Entrada.UsuarioNotificacao usuarioMensagem)
        {
            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(usuarioMensagem.cpfUsuario));

            var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;

            foreach (var lista in listaEscolas)
            {
                foreach (var aluno in lista.Alunos)
                {
                    var usuarioNotificacao = new UsuarioNotificacao();

                    usuarioNotificacao.UsuarioCpf = usuarioMensagem.cpfUsuario;
                    usuarioNotificacao.NotificacaoId = usuarioMensagem.notificacaoId;
                    usuarioNotificacao.UeCodigoEol = aluno.CodigoEscola;
                    usuarioNotificacao.DreCodigoEol = aluno.CodigoDre;
                    usuarioNotificacao.CodigoAlunoEol = aluno.CodigoEol;
                    //  usuarioNotificacao.UsuarioId = pegarUsuarioId
                    // usuarioNotificacao.CriadoPor = 
                    usuarioNotificacao.MensagemVisualizada = usuarioMensagem.mensagemVisualizada;
                    var Notificacao = await mediator.Send(new UsuarioNotificacaoCommand(usuarioNotificacao));
                }
            }
            var notificacao = await mediator.Send(new ObterPorNotificacaoPorIdCommand(usuarioMensagem.notificacaoId));

            notificacao.MensagemVisualizada = usuarioMensagem.mensagemVisualizada;
            return notificacao;
        }
    }
}
