using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel
{
    public class ObterGrupoNotificacaoPorResponsavelCommand : IRequest<IEnumerable>
    {
        public string Cpf { get; set; }

        public ObterGrupoNotificacaoPorResponsavelCommand(string cpf)
        {
            Cpf = cpf;
        }
    }

    public class ObterGrupoNotificacaoPorResponsavelCommandHandler : IRequestHandler<ObterGrupoNotificacaoPorResponsavelCommand, IEnumerable>
    {
        private readonly INotificacaoRepository _repository;
        
        public ObterGrupoNotificacaoPorResponsavelCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable> Handle(ObterGrupoNotificacaoPorResponsavelCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ObterGruposDoResponsavel(request.Cpf);
        }
    }
}