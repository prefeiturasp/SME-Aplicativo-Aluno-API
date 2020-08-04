using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida
{
    public class MarcarMensagemLidaUseCase : IMarcarMensagemLidaUseCase
    {
        public async Task<NotificacaoResposta> Executar(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario)
        {
            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(cpfUsuario));

            if (resposta.Data == null)
                throw new NegocioException("Não foi possivel obter os alunos por escola");

            var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;
            var notificacao = await mediator.Send(new ObterNotificacaoPorIdQuery(usuarioMensagem.NotificacaoId));

            if (notificacao == null)
                throw new NegocioException("Não foi possivel localizar a notificação.");
            if (notificacao.TipoComunicado == TipoComunicado.ALUNO)

            {
                var alunoEol = new Dominio.Entidades.Aluno();

                foreach (var lista in listaEscolas)
                {
                    foreach (var aluno in lista.Alunos)
                    {
                        if (aluno.CodigoEol == usuarioMensagem.CodigoAlunoEol)
                        {
                            alunoEol = aluno;
                            break;
                        }

                    }
                }
                var usuarioNotificacao = new UsuarioNotificacao();

                usuarioNotificacao.UsuarioCpf = cpfUsuario;
                usuarioNotificacao.NotificacaoId = usuarioMensagem.NotificacaoId;
                usuarioNotificacao.DreCodigoEol = long.Parse(alunoEol.CodigoDre);
                usuarioNotificacao.UeCodigoEol = alunoEol.CodigoEscola;
                usuarioNotificacao.CodigoEolAluno = usuarioMensagem.CodigoAlunoEol;
                usuarioNotificacao.UsuarioId = usuarioMensagem.UsuarioId;
                usuarioNotificacao.MensagemVisualizada = usuarioMensagem.MensagemVisualizada;


                await IncluiConfirmacaoDeLeitura(mediator, usuarioNotificacao);
            }

            else
            {
                foreach (var lista in listaEscolas)
                {
                    foreach (var aluno in lista.Alunos)
                    {
                        var usuarioNotificacao = new UsuarioNotificacao
                        {
                            UsuarioCpf = cpfUsuario,
                            NotificacaoId = usuarioMensagem.NotificacaoId,
                            DreCodigoEol = long.Parse(aluno.CodigoDre),
                            UeCodigoEol = aluno.CodigoEscola,
                            CodigoEolAluno = aluno.CodigoEol,
                            UsuarioId = usuarioMensagem.UsuarioId,
                            MensagemVisualizada = usuarioMensagem.MensagemVisualizada,
                        };

                        await IncluiConfirmacaoDeLeitura(mediator, usuarioNotificacao);
                    }
                }
            }

            notificacao.MensagemVisualizada = usuarioMensagem.MensagemVisualizada;
            return notificacao;
        }

        private static async Task IncluiConfirmacaoDeLeitura(IMediator mediator, UsuarioNotificacao usuarioNotificacao)
        {
            usuarioNotificacao.InserirAuditoria();
            var Notificacao = await mediator.Send(new UsuarioNotificacaoCommand(usuarioNotificacao));
        }
    }
}
