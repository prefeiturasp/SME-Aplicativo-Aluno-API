using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterCpfsDeResponsaveisPorDreEUeQueryHandler : IRequestHandler<ObterCpfsDeResponsaveisPorDreEUeQuery, IEnumerable<CpfResponsavelAlunoEol>>
    {
        private readonly IAlunoRepositorio _alunoRepositorio;

        public ObterCpfsDeResponsaveisPorDreEUeQueryHandler(IAlunoRepositorio alunoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio ?? throw new System.ArgumentNullException(nameof(alunoRepositorio));
        }

        public async Task<IEnumerable<CpfResponsavelAlunoEol>> Handle(ObterCpfsDeResponsaveisPorDreEUeQuery request, CancellationToken cancellationToken)
        {
            var cpfsDeResponsavel = await _alunoRepositorio.ObterCpfsDeResponsaveis(request.CodigoDre, request.CodigoUe);
            if (cpfsDeResponsavel == null || !cpfsDeResponsavel.Any())
                throw new NegocioException("Não existem registros de responsáveis para a DRE e UE informadas.");

            return cpfsDeResponsavel;
        }
    }
}
