using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.UnidadeEscolar
{
    public class ObterDadosUnidadeEscolarQueryHandler : IRequestHandler<ObterDadosUnidadeEscolarQuery, UnidadeEscolarResposta>
    {
        private readonly IUnidadeEscolarRepositorio unidadeEscolarRepositorio;

        public ObterDadosUnidadeEscolarQueryHandler(IUnidadeEscolarRepositorio unidadeEscolarRepositorio)
        {
            this.unidadeEscolarRepositorio = unidadeEscolarRepositorio ?? throw new System.ArgumentNullException(nameof(unidadeEscolarRepositorio));
        }

        public async Task<UnidadeEscolarResposta> Handle(ObterDadosUnidadeEscolarQuery request, CancellationToken cancellationToken)
        {
            return await unidadeEscolarRepositorio.ObterDadosUnidadeEscolarPorCodigoUe(request.CodigoUe);
        }
    }
}