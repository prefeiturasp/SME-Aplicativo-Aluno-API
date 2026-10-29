using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.TermosDeUso
{
    public class ExcluirAceiteTermosDeUsoPorCpfCommandHandler : IRequestHandler<ExcluirAceiteTermosDeUsoPorCpfCommand, bool>
    {
        private readonly IAceiteTermosDeUsoRepositorio _repositorio;

        public ExcluirAceiteTermosDeUsoPorCpfCommandHandler(IAceiteTermosDeUsoRepositorio repositorio)
        {
            _repositorio = repositorio ?? throw new System.ArgumentNullException(nameof(repositorio));
        }

        public async Task<bool> Handle(ExcluirAceiteTermosDeUsoPorCpfCommand request, CancellationToken cancellationToken)
        {
            return await _repositorio.ExcluirLogicamentePorCpf(request.CpfUsuario);
        }
    }
}

