using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterUsuarioDetalhesPorCpfQueryHandler : IRequestHandler<ObterUsuarioDetalhesPorCpfQuery, UsuarioDadosDetalhesDto>
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;

        public ObterUsuarioDetalhesPorCpfQueryHandler(IResponsavelEOLRepositorio responsavelEOLRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
        }
        public async Task<UsuarioDadosDetalhesDto> Handle(ObterUsuarioDetalhesPorCpfQuery request, CancellationToken cancellationToken)
        {
            return await responsavelEOLRepositorio.ObterPorCpfParaDetalhes(request.Cpf);
        }
    }
}
