using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ObterTermosDeUsoQueryHandler : IRequestHandler<ObterTermosDeUsoQuery, RetornoTermosDeUsoDto>
    {
        private readonly ITermosDeUsoRepositorio _termosDeUsoRepositorio;

        public ObterTermosDeUsoQueryHandler(ITermosDeUsoRepositorio termosDeUsoRepositorio)
        {
            _termosDeUsoRepositorio = termosDeUsoRepositorio ?? throw new System.ArgumentNullException(nameof(termosDeUsoRepositorio));
        }

        public async Task<RetornoTermosDeUsoDto> Handle(ObterTermosDeUsoQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _termosDeUsoRepositorio.ObterUltimaVersaoTermosDeUso();
            if (retorno == null)
                return null;

            var termosDeUsoDto = MapearObjetoParaDto(retorno);
            return termosDeUsoDto;
        }

        private static RetornoTermosDeUsoDto MapearObjetoParaDto(Dominio.Entidades.TermosDeUso termosDeUso)
        {
            return new RetornoTermosDeUsoDto(termosDeUso.DescricaoTermosDeUso, termosDeUso.DescricaoPoliticaPrivacidade, termosDeUso.Versao, termosDeUso.Id);
        }
    }
}
