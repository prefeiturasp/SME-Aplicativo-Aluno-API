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
    public class ObterDadosAlunosPorDreUeCpfResponsavelQueryHandler : IRequestHandler<ObterDadosAlunosPorDreUeCpfResponsavelQuery, IEnumerable<AlunoRespostaEol>>
    {
        private readonly IAlunoRepositorio _alunoRepositorio;

        public ObterDadosAlunosPorDreUeCpfResponsavelQueryHandler(IAlunoRepositorio alunoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio ?? throw new System.ArgumentNullException(nameof(alunoRepositorio));
        }

        public async Task<IEnumerable<AlunoRespostaEol>> Handle(ObterDadosAlunosPorDreUeCpfResponsavelQuery request, CancellationToken cancellationToken)
        {
            var alunos = await _alunoRepositorio.ObterDadosAlunosPorDreUeCpfResponsavel(request.CodigoDre, request.CodigoUe, request.Cpf);
            if (alunos == null || !alunos.Any())
                throw new NegocioException("Este CPF não consta como responsável de um estudante ativo nesta Unidade Escolar.");

            return alunos;
        }
    }
}
