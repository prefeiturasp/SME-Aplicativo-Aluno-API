using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Comum.Enumeradores;
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

            switch (notificacao.TipoComunicado)
            {
                case TipoComunicado.SME:
                    await ConfirmarLeituraSME(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.DRE:
                    await ConfirmarLeituraDRE(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.UE:
                    await ConfirmarLeituraUE(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.UEMOD:
                    await ConfirmarLeituraUEMOD(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.TURMA:
                    await ConfirmarLeituraTurma(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.ALUNO:
                    await ConfirmarLeituraAlunoEspecifico(mediator, usuarioMensagem, cpfUsuario, listaEscolas);
                    break;
                default:
                    throw new NegocioException("Não identificado o Tipo desta notificação");
            };

            notificacao.MensagemVisualizada = usuarioMensagem.MensagemVisualizada;
            return notificacao;
        }

        private static async Task ConfirmarLeituraTurma(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var turmas = listaEscolas.Where(x => notificacao.Turmas.Any(x => x.CodigoTurma == x.CodigoTurma)).Select(x => x.Alunos);

            if (turmas == null || !turmas.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na turma notificada");

            foreach (var turma in turmas)
                foreach (var aluno in turma)
                    await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraUEMOD(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var modalidades = listaEscolas.Where(x => (notificacao.GruposId?.Any(z => z == x.CodigoGrupo.ToString()) ?? false) && x.Alunos.Any(z => z.CodigoEscola == notificacao.CodigoUe)).Select(x => x.Alunos);

            if (modalidades == null || !modalidades.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na modalidade da escola da notificação");

            foreach (var modalidade in modalidades)
                foreach (var aluno in modalidade)
                    await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraUE(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var ues = listaEscolas.Where(x => x.Alunos.Any(z => z.CodigoEscola.Equals(notificacao.CodigoUe))).Select(x => x.Alunos);

            if (ues == null || !ues.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na escola da notificação");

            foreach (var ue in ues)
                foreach (var aluno in ue)
                    await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraDRE(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var dres = listaEscolas.Where(x => x.Alunos.Any(z => z.CodigoDre.Equals(notificacao.CodigoDre))).Select(x => x.Alunos);

            if (dres == null || !dres.Any())
                throw new NegocioException("Nenhum aluno deste usuário está matriculado na DRE desta Notificação");

            foreach (var dre in dres)
                foreach (var aluno in dre)
                    await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraSME(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var grupos = listaEscolas.Where(x => notificacao.GruposId?.Any(z => z == x.CodigoGrupo.ToString()) ?? false).Select(x => x.Alunos);

            if (grupos == null || !grupos.Any())
                throw new NegocioException("Nenhum aluno deste usuário esta matriculado em um dos grupos da Notificação");

            foreach (var grupo in grupos)
                foreach (var aluno in grupo)
                    await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraAlunoEspecifico(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas)
        {
            var alunoEol = listaEscolas.FirstOrDefault(x => x.Alunos.Any(z => z.CodigoEol == usuarioMensagem.CodigoAlunoEol))?.Alunos.FirstOrDefault(x => x.CodigoEol == usuarioMensagem.CodigoAlunoEol) ?? default;

            if (alunoEol == null)
                throw new NegocioException($"Não encontrado aluno com o codigo {usuarioMensagem.CodigoAlunoEol}");

            await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, alunoEol);
        }

        private static async Task ConfirmarLeitura(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, Dominio.Entidades.Aluno aluno)
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

        private static async Task IncluiConfirmacaoDeLeitura(IMediator mediator, UsuarioNotificacao usuarioNotificacao)
        {
            usuarioNotificacao.InserirAuditoria();

            await mediator.Send(new UsuarioNotificacaoCommand(usuarioNotificacao));
        }
    }
}
