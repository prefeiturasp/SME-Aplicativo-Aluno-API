using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Dominio.Entidades;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf
{
    public class ObterUsuarioPorCpfCommand : IRequest<Dominio.Entidades.Usuario>
    {
        public string Cpf { get; set; }

        public ObterUsuarioPorCpfCommand(string cpf)
        {
            Cpf = cpf;
        }
    }
    
    public class ObterUsuarioPorCpfCommandHandler : IRequestHandler<ObterUsuarioPorCpfCommand, Dominio.Entidades.Usuario>
    {        
        private readonly IUsuarioRepository _repository;
        
        public ObterUsuarioPorCpfCommandHandler(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioPorCpfCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf);
        }
    }
}