using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ObterTermosDeUsoPorCpfQueryHandler : IRequestHandler<ObterTermosDeUsoPorCpfQuery, RetornoTermosDeUsoDto>
    {
        private readonly ITermosDeUsoRepositorio _termosDeUsoRepositorio;

        public ObterTermosDeUsoPorCpfQueryHandler(ITermosDeUsoRepositorio termosDeUsoRepositorio)
        {
            _termosDeUsoRepositorio = termosDeUsoRepositorio ?? throw new System.ArgumentNullException(nameof(termosDeUsoRepositorio));
        }

        public async Task<RetornoTermosDeUsoDto> Handle(ObterTermosDeUsoPorCpfQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _termosDeUsoRepositorio.ObterTermosDeUsoPorCpf(request.CPF);
            if (retorno == null)
                throw new NegocioException("Nenhum termo de uso encontrado para o CPF informado");

            var termosDeUsoDto = MapearObjetoParaDto(retorno);
            return termosDeUsoDto;
        }

        private static RetornoTermosDeUsoDto MapearObjetoParaDto(Dominio.Entidades.TermosDeUso termosDeUso)
        {
            return new RetornoTermosDeUsoDto(termosDeUso.DescricaoTermosDeUso, termosDeUso.DescricaoPoliticaPrivacidade, termosDeUso.Versao, termosDeUso.Id);
        }
    }
}
