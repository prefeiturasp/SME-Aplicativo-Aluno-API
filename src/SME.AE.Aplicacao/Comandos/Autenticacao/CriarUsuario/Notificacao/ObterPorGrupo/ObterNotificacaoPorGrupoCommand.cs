using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;

namespace SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo
{
    public class ObterNotificacaoPorGrupoCommand : IRequest<IEnumerable<NotificacaoPorUsuario>>
    {
        public string Grupo { get; set; }
        public string Cpf { get; }

        public ObterNotificacaoPorGrupoCommand(string grupo, string cpf)
        {
            Grupo = grupo;
            Cpf = cpf;
        }
    }

    public class ObterNotificacaoPorGrupoCommandHandler : IRequestHandler<ObterNotificacaoPorGrupoCommand, 
        IEnumerable<NotificacaoPorUsuario>>
    {
        private readonly INotificacaoRepository _repository;
    
        public ObterNotificacaoPorGrupoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NotificacaoPorUsuario>> Handle
            (ObterNotificacaoPorGrupoCommand request, CancellationToken cancellationToken) => await _repository.ObterPorGrupoUsuario(request.Grupo, request.Cpf);
    }
}