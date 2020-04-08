using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo
{
    public class ObterNotificacaoPorGrupoCommand : IRequest<IEnumerable<Dominio.Entidades.Notificacao>>
    {
        public string Grupo { get; set; }

        public ObterNotificacaoPorGrupoCommand(string grupo)
        {
            Grupo = grupo;
        }
    }

    public class ObterNotificacaoPorGrupoCommandHandler : IRequestHandler<ObterNotificacaoPorGrupoCommand, 
        IEnumerable<Dominio.Entidades.Notificacao>>
    {
        private readonly INotificacaoRepository _repository;
    
        public ObterNotificacaoPorGrupoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Dominio.Entidades.Notificacao>> Handle
            (ObterNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ObterPorGrupo(request.Grupo);
        }
    }
}