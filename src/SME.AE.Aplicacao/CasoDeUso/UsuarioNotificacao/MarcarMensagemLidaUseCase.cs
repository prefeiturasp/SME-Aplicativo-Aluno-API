using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
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
        public async Task<NotificacaoResposta> Executar(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery()
            {
                Cpf = usuarioMensagem.CpfUsuario
            });

            if (usuario == null)
                throw new NegocioException($"Não encontrado usuário com o CPF '{usuarioMensagem.CpfUsuario}'");

            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(usuarioMensagem.CpfUsuario));

            if (resposta.Data == null)
                throw new NegocioException("Não foi possivel obter os alunos por escola");

            var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;
            var notificacao = await mediator.Send(new ObterNotificacaoPorIdQuery(usuarioMensagem.NotificacaoId));

            if (notificacao == null)
                throw new NegocioException("Não foi possivel localizar a notificação.");


            foreach (var lista in listaEscolas.Where(c => notificacao.Grupos.Any(n => n.Codigo == c.CodigoGrupo)))
            {
                foreach (var aluno in lista.Alunos)
                {
                    var usuarioNotificacao = new UsuarioNotificacao
                    {
                        UsuarioCpf = usuario.Cpf,
                        NotificacaoId = usuarioMensagem.NotificacaoId,
                        DreCodigoEol = long.Parse(aluno.CodigoDre),
                        UeCodigoEol = aluno.CodigoEscola,
                        CodigoAlunoEol = aluno.CodigoEol,
                        UsuarioId = usuario.Id,
                        MensagemVisualizada = usuarioMensagem.MensagemVisualizada,
                    };

                    usuarioNotificacao.InserirAuditoria();

                    var Notificacao = await mediator.Send(new UsuarioNotificacaoCommand(usuarioNotificacao));
                }
            }

            notificacao.MensagemVisualizada = usuarioMensagem.MensagemVisualizada;
            return notificacao;
        }
    }
}
