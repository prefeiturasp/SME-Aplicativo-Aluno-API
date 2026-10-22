using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Criar
{
    public class CriarNotificacaoCommandHandler : IRequestHandler<CriarNotificacaoCommand, Dominio.Entidades.Notificacao>
    {
        private readonly INotificacaoRepositorio _repository;

        public CriarNotificacaoCommandHandler(INotificacaoRepositorio repository)
        {
            _repository = repository;
        }

        public async Task<Dominio.Entidades.Notificacao> Handle(CriarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notificacaoCriada = await _repository.Criar(request.Notificacao);

                if (request.Notificacao.TipoComunicado == TipoComunicado.ALUNO)
                    await IncluirNotificacaoAlunos(request);
                else if (request.Notificacao.TipoComunicado == TipoComunicado.MENSAGEM_AUTOMATICA)
                    await IncluirNotificacaoAlunos(request);
                else if (request.Notificacao.TipoComunicado == TipoComunicado.TURMA)
                    await IncluirNotificacaoTurma(request);

                return notificacaoCriada;

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        private async Task IncluirNotificacaoTurma(CriarNotificacaoCommand request)
        {
            foreach (var codigoTurma in request.Notificacao.Turmas)
            {
                var notificacaoTurma = new NotificacaoTurma
                {
                    CodigoTurma = Convert.ToInt64(codigoTurma),
                    NotificacaoId = request.Notificacao.Id
                };
                notificacaoTurma.InserirAuditoria();
                await _repository.InserirNotificacaoTurma(notificacaoTurma);
            }
        }

        private async Task IncluirNotificacaoAlunos(CriarNotificacaoCommand request)
        {
            foreach (var codigoAluno in request.Notificacao.Alunos)
            {
                var notificacaoAluno = new NotificacaoAluno
                {
                    CodigoAluno = Convert.ToInt64(codigoAluno),
                    NotificacaoId = request.Notificacao.Id
                };
                notificacaoAluno.InserirAuditoria();
                await _repository.InserirNotificacaoAluno(notificacaoAluno);
            }
        }
    }
}
