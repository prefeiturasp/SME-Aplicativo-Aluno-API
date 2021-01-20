using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Criar
{
    public class CriarNotificacaoCommandHandler : IRequestHandler<CriarNotificacaoCommand, Unit>
    {
        private readonly INotificacaoRepository _repository;

        public CriarNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CriarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.Criar(request.Notificacao);

                if (request.Notificacao.TipoComunicado == TipoComunicado.ALUNO)
                    await IncluirNotificacaoAlunos(request);

                else if (request.Notificacao.TipoComunicado == TipoComunicado.TURMA)
                    await IncluirNotificacaoTurma(request);

                return Unit.Value;

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
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
