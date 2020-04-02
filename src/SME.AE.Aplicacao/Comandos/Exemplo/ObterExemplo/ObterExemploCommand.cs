using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;

namespace SME.AE.Aplicacao.Comandos.Exemplo.ObterExemplo
{
    public class ObterExemploCommand : IRequest<IEnumerable<string>>
    {
    }

    public class ObterExemploCommandHandler : IRequestHandler<ObterExemploCommand, IEnumerable<string>>
    {
        private readonly IAplicacaoContext _context;
        private readonly IExemploRepository _repository;
        
        public ObterExemploCommandHandler(IAplicacaoContext context, IExemploRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<IEnumerable<string>> Handle(ObterExemploCommand request, CancellationToken cancellationToken)
        {
            return _repository.ObterNomesDeExemplos();
        }
    }
}