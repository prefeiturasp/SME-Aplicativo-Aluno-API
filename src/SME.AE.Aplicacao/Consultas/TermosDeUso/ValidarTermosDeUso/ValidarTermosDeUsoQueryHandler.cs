using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ValidarTermosDeUsoQueryHandler : IRequestHandler<ValidarTermosDeUsoQuery, bool>
    {
        private readonly ITermosDeUsoRepositorio _termosDeUsoRepositorio;

        public ValidarTermosDeUsoQueryHandler(ITermosDeUsoRepositorio termosDeUsoRepositorio)
        {
            _termosDeUsoRepositorio = termosDeUsoRepositorio ?? throw new System.ArgumentNullException(nameof(termosDeUsoRepositorio));
        }

        public async Task<bool> Handle(ValidarTermosDeUsoQuery request, CancellationToken cancellationToken)
        {
            var termosDeUso = await _termosDeUsoRepositorio.ObterPorIdAsync(request.TermoDeUsoId);
            if (termosDeUso == null)
                throw new NegocioException("Não foi possível encontrar os Termos de Uso.");
            return true;
        }

    }
}
