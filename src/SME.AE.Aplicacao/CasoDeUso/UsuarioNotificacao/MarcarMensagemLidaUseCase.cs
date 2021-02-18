using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Utilitarios;
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
                case TipoComunicado.SME_ANO:
                    await ConfirmarLeituraSME_ANO(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.DRE:
                    await ConfirmarLeituraDRE(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
                    break;
                case TipoComunicado.DRE_ANO:
                    await ConfirmarLeituraDRE_ANO(mediator, usuarioMensagem, cpfUsuario, listaEscolas, notificacao);
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
            List<Dominio.Entidades.Aluno> alunos = new List<Dominio.Entidades.Aluno>();

            listaEscolas.ForEach(x =>
            {
                var alunosturma = x.Alunos.Where(z => notificacao.Turmas.Any(y => y.CodigoTurma.Equals(z.CodigoTurma)));

                if (alunosturma != null && alunosturma.Any())
                    alunos.AddRange(alunosturma);
            });

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na turma notificada");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraUEMOD(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var seriesResumidas = notificacao.SeriesResumidas.ToStringEnumerable();
            var seriesResumidasNaoExistem = !seriesResumidas.Any();
            var gruposNaoExistem = !notificacao.GruposId.Any();
            var alunos =
                listaEscolas
                .Where(escola => gruposNaoExistem || notificacao.GruposId.Contains(escola.CodigoGrupo.ToString()))
                .SelectMany(escola => escola.Alunos)
                .Where(aluno => aluno.CodigoEscola == notificacao.CodigoUe.ToString())
                .Where(aluno => (seriesResumidasNaoExistem || seriesResumidas.Contains(aluno.SerieResumida)));

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na modalidade/ano da escola da notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraUE(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            List<Dominio.Entidades.Aluno> alunos = new List<Dominio.Entidades.Aluno>();

            listaEscolas.ForEach(escola =>
            {
                var alunosAdicionar = escola.Alunos.Where(x => x.CodigoEscola.Equals(notificacao.CodigoUe.ToString()));

                //var seriesResumidas = notificacao.SeriesResumidas.ToStringEnumerable();
                //if (seriesResumidas.Any())
                //    alunosAdicionar = alunosAdicionar.Where(aluno => seriesResumidas.Contains(aluno.SerieResumida));

                if (alunosAdicionar == null || !alunosAdicionar.Any())
                    return;

                alunos.AddRange(alunosAdicionar);
            });

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum Aluno está matriculado na escola da notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraDRE(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            List<Dominio.Entidades.Aluno> alunos = new List<Dominio.Entidades.Aluno>();

            listaEscolas.ForEach(escola =>
            {
                if (notificacao.GruposId != null && notificacao.GruposId.Length > 0 && !notificacao.GruposId.Any(grupo => grupo.Equals(escola.CodigoGrupo.ToString())))
                    return;

                var adicionar = escola.Alunos.Where(aluno => aluno.CodigoDre.Equals(notificacao.CodigoDre.ToString()));

                if (adicionar == null || !adicionar.Any())
                    return;

                alunos.AddRange(adicionar);
            });

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum aluno deste usuário está matriculado na DRE desta Notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraDRE_ANO(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var seriesResumidas = notificacao.SeriesResumidas.ToStringEnumerable();
            var seriesResumidasNaoExistem = !seriesResumidas.Any();
            var gruposNaoExistem = !notificacao.GruposId.Any();
            var alunos =
                listaEscolas
                .Where(escola => gruposNaoExistem || notificacao.GruposId.Contains(escola.CodigoGrupo.ToString()))
                .SelectMany(escola => escola.Alunos)
                .Where(aluno => aluno.CodigoDre == notificacao.CodigoDre.ToString())
                .Where(aluno => (seriesResumidasNaoExistem || seriesResumidas.Contains(aluno.SerieResumida)));

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum aluno deste usuário está matriculado na DRE/Ano desta Notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraSME(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            List<Dominio.Entidades.Aluno> alunos = new List<Dominio.Entidades.Aluno>();

            listaEscolas.ForEach(escola =>
            {
                var alunosAdicionar = notificacao.GruposId.Any(grupo => grupo.Equals(escola.CodigoGrupo.ToString())) ? escola.Alunos : default;

                if (alunosAdicionar == null || !alunosAdicionar.Any())
                    return;

                alunos.AddRange(alunosAdicionar);
            });

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum aluno deste usuário está matriculado no grupo desta Notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraSME_ANO(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas, NotificacaoResposta notificacao)
        {
            var seriesResumidas = notificacao.SeriesResumidas.ToStringEnumerable();
            var seriesResumidasNaoExistem = !seriesResumidas.Any();
            var alunos = 
                listaEscolas
                .Where(escola => notificacao.GruposId.Contains(escola.CodigoGrupo.ToString()))
                .SelectMany(escola => escola.Alunos)
                .Where(alunos => (seriesResumidasNaoExistem || seriesResumidas.Contains(alunos.SerieResumida)));

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Nenhum aluno deste usuário está matriculado no grupo/ano desta Notificação");

            foreach (var aluno in alunos)
                await ConfirmarLeitura(mediator, usuarioMensagem, cpfUsuario, aluno);
        }

        private static async Task ConfirmarLeituraAlunoEspecifico(IMediator mediator, UsuarioNotificacaoDto usuarioMensagem, string cpfUsuario, IEnumerable<ListaEscola> listaEscolas)
        {
            List<Dominio.Entidades.Aluno> alunos = new List<Dominio.Entidades.Aluno>();

            listaEscolas.ForEach(escola =>
            {
                var alunosAdicionar = escola.Alunos.Where(x => x.CodigoEol == usuarioMensagem.CodigoAlunoEol);

                if (alunosAdicionar == null || !alunosAdicionar.Any())
                    return;

                alunos.AddRange(alunosAdicionar);
            });

            var alunoEol = alunos.FirstOrDefault();

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
                CodigoEolTurma = aluno.CodigoTurma
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
