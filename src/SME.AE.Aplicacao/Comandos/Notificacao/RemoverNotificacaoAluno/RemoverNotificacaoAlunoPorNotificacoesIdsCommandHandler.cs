using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoAlunoPorNotificacoesIdsCommandHandler : IRequestHandler<RemoverNotificacaoAlunoPorNotificacoesIdsCommand, bool>
    {
        private readonly INotificacaoAlunoRepositorio repositorio;

        public RemoverNotificacaoAlunoPorNotificacoesIdsCommandHandler(INotificacaoAlunoRepositorio repositorio)
        {
            this.repositorio = repositorio ?? throw new System.ArgumentNullException(nameof(repositorio));
        }


        public async Task<bool> Handle(RemoverNotificacaoAlunoPorNotificacoesIdsCommand request, CancellationToken cancellationToken)
        {
            return await repositorio.RemoverPorNotificacoesIds(request.Ids);
        }
    }
}
