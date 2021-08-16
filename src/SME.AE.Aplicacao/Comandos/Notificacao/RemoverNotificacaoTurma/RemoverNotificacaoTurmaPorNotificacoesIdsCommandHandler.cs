using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoTurmaPorNotificacoesIdsCommandHandler : IRequestHandler<RemoverNotificacaoTurmaPorNotificacoesIdsCommand, bool>
    {
        private readonly INotificacaoTurmaRepositorio repositorio;

        public RemoverNotificacaoTurmaPorNotificacoesIdsCommandHandler(INotificacaoTurmaRepositorio repositorio)
        {
            this.repositorio = repositorio ?? throw new System.ArgumentNullException(nameof(repositorio));
        }


        public async Task<bool> Handle(RemoverNotificacaoTurmaPorNotificacoesIdsCommand request, CancellationToken cancellationToken)
        {
            return await repositorio.RemoverPorNotificacoesIds(request.Ids);
        }
    }
}
