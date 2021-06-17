using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterDadosResumidosReponsavelPorCpfQueryHandler : IRequestHandler<ObterDadosResumidosReponsavelPorCpfQuery, ResponsavelAlunoEolResumidoDto>
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;

        public ObterDadosResumidosReponsavelPorCpfQueryHandler(IResponsavelEOLRepositorio responsavelEOLRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
        }

        public async Task<ResponsavelAlunoEolResumidoDto> Handle(ObterDadosResumidosReponsavelPorCpfQuery request, CancellationToken cancellationToken)
        {
            var responsavel = await responsavelEOLRepositorio.ObterDadosResumidosReponsavelPorCpf(request.Cpf);

            if (responsavel == null)
                throw new NegocioException("Responsável não encontrado!");

            return responsavel;
        }
    }
}
