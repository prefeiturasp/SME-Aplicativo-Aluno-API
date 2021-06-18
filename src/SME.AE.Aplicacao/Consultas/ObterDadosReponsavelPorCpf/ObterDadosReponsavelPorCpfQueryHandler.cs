using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterDadosReponsavelPorCpfQueryHandler : IRequestHandler<ObterDadosReponsavelPorCpfQuery, IEnumerable<ResponsavelAlunoDetalhadoEolDto>>
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;

        public ObterDadosReponsavelPorCpfQueryHandler(IResponsavelEOLRepositorio responsavelEOLRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
        }

        public async Task<IEnumerable<ResponsavelAlunoDetalhadoEolDto>> Handle(ObterDadosReponsavelPorCpfQuery request, CancellationToken cancellationToken)
        {
            var responsavel = await responsavelEOLRepositorio.ObterDadosReponsavelPorCpf(request.Cpf);

            if (responsavel == null)
                throw new NegocioException("Responsável não encontrado!");

            return responsavel;
        }
    }
}
