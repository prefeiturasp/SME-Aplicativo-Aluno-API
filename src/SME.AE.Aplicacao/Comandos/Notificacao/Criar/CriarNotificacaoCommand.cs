using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Criar
{
    public class CriarNotificacaoCommand : IRequest<Dominio.Entidades.Notificacao>
    {
        public Dominio.Entidades.Notificacao Notificacao { get; set; }

        public CriarNotificacaoCommand(Dominio.Entidades.Notificacao notificacao)
        {
            Notificacao = notificacao;
        }
    }

    public class CriarNotificacaoCommandHandler : IRequestHandler<CriarNotificacaoCommand, Dominio.Entidades.Notificacao>
    {
        private readonly INotificacaoRepository _repository;
        
        public CriarNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Dominio.Entidades.Notificacao> Handle
            (CriarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var resultado = await _repository.Criar(request.Notificacao);
                return resultado;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
